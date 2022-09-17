package viewmodel

import java.io.File

class OutputHelper(
    val file: File
) {
    class NoCategoryOrDifficultException(msg: String) : Exception(msg)

    data class QuestionAndAnswer(
        val id: Int,
        val question: String,
        val answer: String,
        val category: String,
        val difficulty: String
    )

    companion object {
        val specialCharacters = Regex("""[,\s]+""")

        fun getSafeString(str: String): String {
            return str
                .replace("\\", "\\\\")
                .replace("\n", "\\n")
        }

        fun isMetaDataEquals(one: OutputHelper, two: OutputHelper): Boolean {
            return one.categories == two.categories &&
                    one.difficulties == two.difficulties &&
                    one.categoryWeights == two.categoryWeights &&
                    one.difficultyWeights == two.difficultyWeights
        }
    }

    // 为什么不使用 HashMap/Dictionary？
    // 他们是无序的，但我需要有序列表
    var categories: MutableList<String> = mutableListOf()
        private set
    var categoryWeights: MutableList<Int> = mutableListOf()
        private set
    var difficulties: MutableList<String> = mutableListOf()
        private set
    var difficultyWeights: MutableList<Int> = mutableListOf()
        private set

    private val questionsAndAnswers: MutableList<QuestionAndAnswer> = mutableListOf()

    init {
        if (!file.exists()) {
            file.createNewFile()
        } else {
            // 一行一行读取
            // 编码 UFT8
            var lineNum = 0
            file.readLines().forEach {
                if (it.isNotBlank()) {
                    ++lineNum
                    // 前四行储存版块和难度数据
                    when (lineNum) {
                        // 第一行储存版块名称，以英文逗号为分隔
                        1 -> {
                            categories = it.split(',').toMutableList()
                        }
                        // 每个版块的权重，与版块名称一一对应
                        2 -> {
                            categoryWeights = it.split(',').map { str -> str.toInt() }.toMutableList()
                        }
                        // 难度的名称
                        3 -> {
                            difficulties = it.split(',').toMutableList()
                        }
                        // 难度的权重
                        4 -> {
                            difficultyWeights = it.split(',').map { str -> str.toInt() }.toMutableList()
                        }
                        // 下面是问答题数据
                        else -> {
                            // 以 \! 作为分隔题目、答案、版块编号、难度编号的字符串
                            val split = it.split("\\!")

                            // 复原被转义的 \ 和换行
                            val question = split[0].replace("\\n", "\n").replace("\\\\", "\\")
                            val answer = split[1].replace("\\n", "\n").replace("\\\\", "\\")
                            // 根据编号依次获得版块和难度
                            val category = categories[split[2].toInt()]
                            val difficulty = difficulties[split[3].toInt()]

                            val q = QuestionAndAnswer(
                                lineNum - 5,
                                question,
                                answer,
                                category,
                                difficulty
                            )

                            questionsAndAnswers.add(q)
                        }
                    }
                }
            }
        }
    }

    private fun getQuestionAndAnswerString(q: QuestionAndAnswer): String {
        val categoryId = categories.indexOf(q.category)
        val difficultyId = difficulties.indexOf(q.difficulty)

        if (categoryId == -1) {
            throw NoCategoryOrDifficultException(q.category)
        } else if (difficultyId == -1) {
            throw NoCategoryOrDifficultException(q.difficulty)
        }

        return "${getSafeString(q.question)}\\!${getSafeString(q.answer)}\\!$categoryId\\!$difficultyId\n"
    }

    fun addAll(another: OutputHelper) {
        // 直接断言他俩元数据相等
        another.questionsAndAnswers.forEach {
            questionsAndAnswers.add(it.copy(id = questionsAndAnswers.size))
        }

        save()
    }

    fun getQuestionsAndAnswers(): List<QuestionAndAnswer> = questionsAndAnswers

    fun addQuestionAndAnswerAndSave(q: QuestionAndAnswer) {
        if (!categories.contains(q.category)) {
            throw NoCategoryOrDifficultException(q.category)
        } else if (!difficulties.contains(q.difficulty)) {
            throw NoCategoryOrDifficultException(q.difficulty)
        }

        questionsAndAnswers.add(q)

        file.appendText(getQuestionAndAnswerString(q))
    }

    fun updateById(id: Int, newQ: QuestionAndAnswer) {
        questionsAndAnswers[id] = newQ
    }

    fun deleteQuestionAndAnswerById(id: Int) {
        for (i in questionsAndAnswers.indices) {
            if (i > id) {
                questionsAndAnswers[i - 1] = questionsAndAnswers[i - 1].copy(id = i - 1)
            } else if (i == id) {
                questionsAndAnswers.removeAt(i)
            } else {
                continue
            }
        }
    }

    /**
     * 重新保存，消耗大量 CPU 和 IO
     */
    fun save() {
        val tmpFile = File.createTempFile("temp-output-", "")
        val writer = tmpFile.writer()

        // 第一步：保存 版块 和 难度 数据
        writer.write(categories.joinToString(","))
        writer.write("\n")
        writer.write(categoryWeights.joinToString(","))
        writer.write("\n")
        writer.write(difficulties.joinToString(","))
        writer.write("\n")
        writer.write(difficultyWeights.joinToString(","))
        writer.write("\n")

        // 第二步：保存 QA 数据
        questionsAndAnswers.forEach {
            val str = getQuestionAndAnswerString(it)
            writer.write(str)
        }

        // 完成！
        writer.flush()
        writer.close()
        file.delete()
        tmpFile.copyTo(file)
        tmpFile.delete()
    }
}
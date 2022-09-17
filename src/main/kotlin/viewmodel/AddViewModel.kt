package viewmodel

import helper

class AddViewModel {

    fun getCategoriesWithWeights(): List<Pair<String, Int>> =
        List(helper.categories.size) { Pair(helper.categories[it], helper.categoryWeights[it]) }

    fun getDifficultiesWithWeights(): List<Pair<String, Int>> =
        List(helper.difficulties.size) { Pair(helper.difficulties[it], helper.difficultyWeights[it]) }

    companion object {
        val re = Regex("""（\d+）$""")
    }

    fun removeBrackets(str: String): String {
        return re.replace(str, "")
    }
}
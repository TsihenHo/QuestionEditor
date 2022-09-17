package page

import androidx.compose.desktop.ui.tooling.preview.Preview
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.focus.onFocusChanged
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.unit.dp
import helper
import viewmodel.AddViewModel
import viewmodel.OutputHelper

private val model: AddViewModel by lazy { AddViewModel() }
private val regexForOptions = Regex(
    """(\s[A-F])[ 　.、)）]*""", RegexOption.MULTILINE
)

@Composable
@Preview
fun AddPage() {
    var question by remember { mutableStateOf("") }
    var answer by remember { mutableStateOf("") }
    var selectedCategory by remember { mutableStateOf("") }
    var selectedDifficulty by remember { mutableStateOf("") }
    var questionError by remember { mutableStateOf(false) }
    var answerError by remember { mutableStateOf(false) }

    Column(modifier = Modifier.fillMaxSize()) {
        var needSelectCategory by remember { mutableStateOf(false) }
        var needSelectDifficulty by remember { mutableStateOf(false) }

        OutlinedTextField(
            value = question,
            onValueChange = {
                question = it
            },
            label = { Text("题目") },
            shape = RoundedCornerShape(8.dp),
            placeholder = { Text("温馨提示：下面的“版块选择”“难度选择”可以使用 Shift + 鼠标滚轮进行滚动！") },
            isError = questionError,
            modifier = Modifier.weight(0.4f).fillMaxWidth().padding(8.dp).onFocusChanged {
                if (!it.isFocused && question.isNotEmpty()) {
                    // 优化选项
                    question = regexForOptions.replace(question) { res ->
                        "${res.groupValues[1]}. "
                    }
                }
            }
        )

        OutlinedTextField(
            value = answer,
            onValueChange = { answer = it },
            label = { Text("答案") },
            modifier = Modifier.weight(0.2f).fillMaxWidth().padding(8.dp),
            shape = RoundedCornerShape(8.dp),
            isError = answerError
        )

        Row(modifier = Modifier.weight(0.1f).fillMaxSize().padding(8.dp)) {
            Box(contentAlignment = Alignment.Center, modifier = Modifier.fillMaxHeight()) {
                Text("版块选择：", color = if (needSelectCategory) MaterialTheme.colors.error else Color.Unspecified)
            }

            val horizontal = rememberScrollState()

            Row(modifier = Modifier.horizontalScroll(horizontal)) {
                RadioButtonGroup(
                    model.getCategoriesWithWeights()
                        .filter { it.second > 0 }
                        .map { it.first },
                    null
                ) {
                    selectedCategory = it
                    needSelectCategory = false
                }
            }
        }

        Row(modifier = Modifier.weight(0.1f).fillMaxSize().padding(8.dp)) {
            Box(contentAlignment = Alignment.Center, modifier = Modifier.fillMaxHeight()) {
                Text("难度选择：", color = if (needSelectDifficulty) MaterialTheme.colors.error else Color.Unspecified)
            }

            val horizontal = rememberScrollState()

            Row(modifier = Modifier.horizontalScroll(horizontal)) {
                RadioButtonGroup(
                    model.getDifficultiesWithWeights()
                        .filter { it.second > 0 }
                        .map { it.first },
                    null
                ) {
                    selectedDifficulty = it
                    needSelectDifficulty = false
                }
            }
        }

        var btnText by remember { mutableStateOf("添加题目") }
        var enableBtn by remember { mutableStateOf(true) }

        Row(modifier = Modifier.weight(0.2f).padding(8.dp)) {
            Button(
                onClick = {
                    questionError = question.isEmpty()
                    answerError = answer.isEmpty()
                    needSelectDifficulty = selectedDifficulty.isEmpty()
                    needSelectCategory = selectedCategory.isEmpty()

                    if (questionError || answerError || needSelectDifficulty || needSelectCategory) {
                        return@Button
                    }

                    helper.addQuestionAndAnswerAndSave(
                        OutputHelper.QuestionAndAnswer(
                            helper.getQuestionsAndAnswers().size,
                            question,
                            answer,
                            selectedCategory,
                            selectedDifficulty
                        )
                    )

                    btnText = "成功"
                    enableBtn = false
                    question = ""
                    answer = ""
                    Thread {
                        Thread.sleep(1500)
                        btnText = "添加题目"
                        enableBtn = true
                    }
                        .start()
                },
                enabled = enableBtn
            ) {
                Text(btnText)
            }
        }
    }
}

@Composable
fun RadioButtonGroup(texts: List<String>, defaultChecked: String? = null, onCheckedChange: (String) -> Unit) {
    val checked = remember { mutableStateOf(defaultChecked) }

    texts.forEach {
        TextButton(
            enabled = checked.value != it,
            onClick = {
                checked.value = it
                onCheckedChange(it)
            }
        ) {
            Text(text = it)
        }

        Spacer(modifier = Modifier.width(4.dp))
    }
}
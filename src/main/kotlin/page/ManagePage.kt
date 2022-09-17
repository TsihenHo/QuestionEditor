package page

import androidx.compose.foundation.ScrollbarAdapter
import androidx.compose.foundation.VerticalScrollbar
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.rememberLazyListState
import androidx.compose.material.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.SpanStyle
import androidx.compose.ui.text.buildAnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.text.withStyle
import androidx.compose.ui.unit.dp
import helper
import mainWindow
import viewmodel.OutputHelper
import java.util.regex.PatternSyntaxException
import javax.swing.JOptionPane

private lateinit var totalList: MutableList<OutputHelper.QuestionAndAnswer>

@ExperimentalMaterialApi
@Composable
fun ManagePage() {

    totalList = remember {
        mutableStateListOf<OutputHelper.QuestionAndAnswer>().apply {
            addAll(helper.getQuestionsAndAnswers())
        }
    }
    var filterRule by remember {
        mutableStateOf(
            FilterRule(
                Regex("", RegexOption.MULTILINE),
                Regex("", RegexOption.MULTILINE),
                helper.categories.toSet(),
                helper.difficulties.toSet()
            )
        )
    }
    val filteredList = filterRule.applyRule(totalList)

    var showEditDialog by remember { mutableStateOf(false) }
    var clickedId by remember { mutableStateOf(-1) }

    if (showEditDialog) {
        EditDialog(
            sourceQ = totalList[clickedId],
            onCancelButtonClick = { showEditDialog = false },
            onDeleteButtonClick = {
                helper.deleteQuestionAndAnswerById(clickedId)
                totalList.clear()
                totalList.addAll(helper.getQuestionsAndAnswers())
                System.gc()
                showEditDialog = false
            },
            onSaveButtonClick = { newQ ->
                totalList[clickedId] = newQ
                helper.updateById(clickedId, newQ)

                showEditDialog = false
            }
        )
    }

    var showFilterDialog by remember { mutableStateOf(false) }
    if (showFilterDialog) {
        FilterDialog(
            onSaveButtonClick = {
                filterRule = it
                showFilterDialog = false
            },
            onCancelButtonClick = { showFilterDialog = false },
            filterRule
        )
    }

    Column(modifier = Modifier.fillMaxSize().padding(8.dp)) {
        Text(
            "警告：在此页作出的所有更改必须点击“全部保存”按钮后才生效。",
            style = MaterialTheme.typography.body1,
            color = MaterialTheme.colors.secondary
        )
        Spacer(modifier = Modifier.height(16.dp))

        Row(modifier = Modifier.align(Alignment.CenterHorizontally).padding(4.dp)) {
            TextButton(
                onClick = { showFilterDialog = true },
                modifier = Modifier.padding(0.dp, 0.dp, 8.dp, 0.dp)
            ) {
                Text("筛选")
            }

            Text(buildAnnotatedString {
                withStyle(style = SpanStyle(color = MaterialTheme.colors.secondary)) {
                    append("总条数：")
                    withStyle(SpanStyle(fontWeight = FontWeight.Bold)) {
                        append(totalList.size.toString())
                    }
                    append("    当前条数：")
                    withStyle(SpanStyle(fontWeight = FontWeight.Bold)) {
                        append(filteredList.size.toString())
                    }
                }
            }, modifier = Modifier.padding(ButtonDefaults.TextButtonContentPadding))
        }

        Box(modifier = Modifier.fillMaxSize()) {
            val state = rememberLazyListState()
            LazyColumn(
                modifier = Modifier.fillMaxSize(),
                state = state
            ) {
                filteredList.forEach {
                    item {
                        ShowQuestionAndAnswer(
                            it,
                            onClick = {
                                clickedId = it.id
                                showEditDialog = true
                            })
                    }
                }
            }

            VerticalScrollbar(ScrollbarAdapter(state), modifier = Modifier.align(Alignment.CenterEnd))
        }
    }
}

@Composable
fun ShowQuestionAndAnswer(q: OutputHelper.QuestionAndAnswer, onClick: () -> Unit) {
    Row(modifier = Modifier.fillMaxWidth().clickable { onClick() }.padding(4.dp)) {
        Text(q.question, Modifier.weight(0.6f), overflow = TextOverflow.Ellipsis, maxLines = 1)
        Text(q.answer, Modifier.weight(0.15f), overflow = TextOverflow.Ellipsis, maxLines = 1)
        Text(q.category, Modifier.weight(0.125f), overflow = TextOverflow.Ellipsis, maxLines = 1)
        Text(q.difficulty, Modifier.weight(0.125f), overflow = TextOverflow.Ellipsis, maxLines = 1)
    }
}

@Composable
fun MyDropdownMenu(text: String, options: List<String>, onClick: (String) -> Unit) {
    val expand = remember { mutableStateOf(false) }
    val selected = remember { mutableStateOf(text) }

    TextButton({ expand.value = !expand.value }) {
        Text(selected.value)
    }
    DropdownMenu(expanded = expand.value, onDismissRequest = { expand.value = false }) {
        options.forEach {
            DropdownMenuItem(onClick = {
                onClick(it)
                selected.value = it
                expand.value = false
            }) {
                Text(it)
            }
        }
    }
}

@ExperimentalMaterialApi
@Composable
fun EditDialog(
    onSaveButtonClick: (OutputHelper.QuestionAndAnswer) -> Unit,
    onCancelButtonClick: () -> Unit,
    onDeleteButtonClick: () -> Unit,
    sourceQ: OutputHelper.QuestionAndAnswer
) {
    val question = remember { mutableStateOf(sourceQ.question) }
    val answer = remember { mutableStateOf(sourceQ.answer) }
    val category = remember { mutableStateOf(sourceQ.category) }
    val difficulty = remember { mutableStateOf(sourceQ.difficulty) }

    AlertDialog(
        onDismissRequest = {},
        title = { Text("编辑题目") },
        text = {
            Column {
                OutlinedTextField(
                    value = question.value,
                    onValueChange = { question.value = it },
                    label = { Text("题目") },
                    modifier = Modifier.padding(8.dp)
                )
                OutlinedTextField(
                    value = answer.value,
                    onValueChange = { answer.value = it },
                    label = { Text("答案") },
                    modifier = Modifier.padding(8.dp)
                )
                Row(modifier = Modifier.fillMaxWidth().padding(8.dp)) {
                    MyDropdownMenu(category.value, helper.categories, onClick = { category.value = it })
                    MyDropdownMenu(difficulty.value, helper.difficulties, onClick = { difficulty.value = it })

                    Box(contentAlignment = Alignment.CenterEnd, modifier = Modifier.fillMaxWidth()) {
                        Button(
                            onClick = {
                                onDeleteButtonClick()
                            },
                            colors = ButtonDefaults.buttonColors(backgroundColor = MaterialTheme.colors.error)
                        ) {
                            Text("删除")
                        }
                    }
                }
            }
        },
        confirmButton = {
            TextButton({
                val newQ = sourceQ.copy(
                    question = question.value,
                    answer = answer.value,
                    difficulty = difficulty.value,
                    category = category.value
                )
                onSaveButtonClick(newQ)
            }, enabled = question.value.isNotEmpty() && answer.value.isNotEmpty()) {
                Text("保存")
            }
        },
        dismissButton = {
            TextButton({
                onCancelButtonClick()
            }) {
                Text("不保存")
            }
        },
        modifier = Modifier.requiredWidth(480.dp)
    )
}

data class FilterRule(
    val questionRegex: Regex,
    val answerRegex: Regex,
    val categories: Set<String>,
    val difficulties: Set<String>
) {
    fun applyRule(sourceList: List<OutputHelper.QuestionAndAnswer>): List<OutputHelper.QuestionAndAnswer> {
        return sourceList.filter {
            questionRegex.containsMatchIn(it.question) &&
                    answerRegex.containsMatchIn(it.answer) &&
                    categories.contains(it.category) &&
                    difficulties.contains(it.difficulty)
        }
            .toList()
    }
}

@Composable
fun CheckBoxGroup(
    texts: List<String>,
    defaultChecked: Set<Int>,
    onCheckedChange: (Set<String>) -> Unit
) {
    val checked = remember { mutableStateListOf<Int>().apply { addAll(defaultChecked.toMutableSet()) } }

    texts.forEach {
        val index = texts.indexOf(it)
        TextButton(
            onClick = {
                if (checked.contains(index)) {
                    checked.remove(index)
                } else {
                    checked.add(index)
                }

                onCheckedChange(checked.map { index2 -> texts[index2] }.toSet())
            },
            colors = ButtonDefaults.textButtonColors(
                contentColor =
                if (checked.contains(index))
                    MaterialTheme.colors.primary
                else
                    MaterialTheme.colors.onSurface.copy(alpha = ContentAlpha.disabled)
            )
        ) {
            Text(it)
        }
    }

    // 什么，你问我下面这个代码有什么用？
    // 你把它删掉就知道它有什么用了
    // 原因：由于玄学，用户前两次对 UI 的操作会无法展示出来，而后面的操作却能够正常反馈
    // 所以这里代替用户完成前两次操作，这样就没问题了
    if (checked.remove(0)) {
        checked.add(0)
    } else {
        checked.add(0)
        checked.remove(0)
    }
}

@ExperimentalMaterialApi
@Composable
fun FilterDialog(
    onSaveButtonClick: (FilterRule) -> Unit,
    onCancelButtonClick: () -> Unit,
    defaultRule: FilterRule
) {
    var questionRegexString by remember { mutableStateOf(defaultRule.questionRegex.pattern) }
    var answerRegexString by remember { mutableStateOf(defaultRule.answerRegex.pattern) }
    var categories = defaultRule.categories
    var difficulties = defaultRule.difficulties

    AlertDialog(
        onDismissRequest = {},
        title = { Text("筛选") },
        text = {
            Column {
                OutlinedTextField(
                    value = questionRegexString,
                    onValueChange = { questionRegexString = it },
                    label = { Text("题目条件") },
                    placeholder = { Text("使用正则表达式来匹配题目") },
                    modifier = Modifier.padding(8.dp)
                )

                OutlinedTextField(
                    value = answerRegexString,
                    onValueChange = { answerRegexString = it },
                    label = { Text("答案条件") },
                    placeholder = { Text("使用正则表达式来匹配答案") },
                    modifier = Modifier.padding(8.dp)
                )

                Row(modifier = Modifier.padding(8.dp)) {
                    CheckBoxGroup(helper.categories, categories.map { helper.categories.indexOf(it) }.toSet(), onCheckedChange = {
                        categories = it
                    })
                }

                Row(modifier = Modifier.padding(8.dp)) {
                    CheckBoxGroup(helper.difficulties, difficulties.map { helper.difficulties.indexOf(it) }.toSet(), onCheckedChange = {
                        difficulties = it
                    })
                }
            }
        },
        confirmButton = {
            TextButton(
                onClick = {
                    try {
                        val rule = FilterRule(
                            Regex(questionRegexString, RegexOption.MULTILINE),
                            Regex(answerRegexString, RegexOption.MULTILINE),
                            categories,
                            difficulties
                        )

                        onSaveButtonClick(rule)
                    } catch (e: PatternSyntaxException) {
                        JOptionPane.showMessageDialog(
                            mainWindow.window,
                            e.localizedMessage,
                            "正则表达式语法错误",
                            JOptionPane.ERROR_MESSAGE
                        )
                    }
                }
            ) {
                Text("应用")
            }
        },
        dismissButton = {
            TextButton(onClick = onCancelButtonClick) {
                Text("取消")
            }
        },
        modifier = Modifier.requiredWidth(480.dp)
    )
}
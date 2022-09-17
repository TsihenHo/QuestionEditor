package page

import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.*
import androidx.compose.runtime.*
import androidx.compose.ui.ExperimentalComposeUiApi
import androidx.compose.ui.Modifier
import androidx.compose.ui.input.key.*
import androidx.compose.ui.unit.dp
import helper
import viewmodel.OutputHelper

@ExperimentalComposeUiApi
@ExperimentalMaterialApi
@Composable
fun DifficultyPage() {
    val showDialog = remember { mutableStateOf(false) }
    if (showDialog.value) {
        var name by remember { mutableStateOf("") }
        var weight by remember { mutableStateOf("") }
        var myTitle by remember { mutableStateOf("编辑/新增难度") }

        val action = action@{
            val weightInt = weight.toIntOrNull()
            if (weightInt == null || weightInt < 0) {
                myTitle = "自然数！！看不见吗！！！"
                return@action
            }
            if (name.isEmpty()) {
                myTitle = "有东西没输呢！"
                return@action
            }
            val i = helper.difficulties.indexOf(name)
            if (i == -1) {
                helper.difficulties.add(name)
                helper.difficultyWeights.add(weightInt)
            } else {
                helper.difficultyWeights[i] = weightInt
            }
            showDialog.value = false
        }

        AlertDialog(
            onDismissRequest = { },
            title = { Text(myTitle) },
            text = {
                Column {
                    Spacer(modifier = Modifier.height(8.dp))
                    Text("提示：暂不支持删除难度，把权重置零可以间接解决，并且在添加题目时自动隐藏")
                    Spacer(modifier = Modifier.height(8.dp))

                    OutlinedTextField(
                        value = name,
                        onValueChange = {
                            name = OutputHelper.specialCharacters.replace(it, "")
                        },
                        label = { Text("难度名") },
                        placeholder = { Text("比如“简单”“困难”“专家”") },
                        singleLine = true,
                        shape = RoundedCornerShape(8.dp),
                        modifier = Modifier.onPreviewKeyEvent {
                            when (it.key) {
                                Key.Enter, Key.NumPadEnter -> {
                                    action()
                                    true
                                }

                                else -> false
                            }
                        }

                    )
                    Spacer(modifier = Modifier.height(8.dp))
                    OutlinedTextField(
                        value = weight,
                        onValueChange = { weight = it },
                        label = { Text("权重") },
                        singleLine = true,
                        placeholder = { Text("必须是自然数！") },
                        shape = RoundedCornerShape(8.dp),
                        modifier = Modifier.onPreviewKeyEvent {
                            when (it.key) {
                                Key.Enter, Key.NumPadEnter -> {
                                    action()
                                    true
                                }

                                else -> false
                            }
                        }
                    )
                }
            },
            confirmButton = {
                TextButton(
                    onClick = action
                ) {
                    Text("确定")
                }
            },
            dismissButton = {
                TextButton(
                    onClick = { showDialog.value = false }
                ) {
                    Text("关闭")
                }
            }
        )
    }

    Column {
        Text(text = "当前所有难度：")
        List(helper.difficulties.size) { "${helper.difficulties[it]}（${helper.difficultyWeights[it]}）" }
            .forEach {
                Spacer(modifier = Modifier.height(8.dp))
                Text(it)
            }

        Spacer(modifier = Modifier.height(8.dp))
        Button(onClick = { showDialog.value = true }) {
            Text("编辑/添加难度")
        }
    }
}
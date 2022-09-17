// Copyright 2000-2021 JetBrains s.r.o. and contributors. Use of this source code is governed by the Apache 2.0 license that can be found in the LICENSE file.
import androidx.compose.desktop.ui.tooling.preview.Preview
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.selection.SelectionContainer
import androidx.compose.material.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.ExperimentalComposeUiApi
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.ui.window.FrameWindowScope
import androidx.compose.ui.window.Window
import androidx.compose.ui.window.application
import page.*
import viewmodel.MainViewModel
import viewmodel.MainViewModel.Page.*
import viewmodel.OutputHelper
import java.io.File
import javax.swing.JFileChooser
import javax.swing.JOptionPane
import javax.swing.UIManager

lateinit var helper: OutputHelper
lateinit var mainWindow: FrameWindowScope
private val model: MainViewModel by lazy { MainViewModel() }

@ExperimentalComposeUiApi
@ExperimentalMaterialApi
@Composable
@Preview
fun App() {
    MaterialTheme {
        Row(modifier = Modifier.fillMaxSize().padding(16.dp)) {
            // 左侧按键列表
            Column(modifier = Modifier.fillMaxHeight().weight(0.15f)) {
                ButtonClickToPage(Welcome, "欢迎")
                ButtonClickToPage(Category, "版块管理")
                ButtonClickToPage(Difficulty, "难度管理")
                ButtonClickToPage(Manage, "题目管理")
                ButtonClickToPage(Add, "添加题目")
                ButtonClickToPage(About, "帮助关于")

                var btnText by remember { mutableStateOf("全部保存") }
                var enableBtn by remember { mutableStateOf(true) }

                Box(modifier = Modifier.fillMaxHeight(), contentAlignment = Alignment.BottomCenter) {
                    Column {
                        Button(
                            onClick = {
                                val fileChooser = JFileChooser("/").apply {
                                    fileSelectionMode = JFileChooser.FILES_ONLY
                                    dialogTitle = "选择另一个数据文件"
                                    approveButtonText = "选择"
                                    approveButtonToolTipText = "选择另一个数据文件来与当前打开的文件合并"
                                }
                                fileChooser.showOpenDialog(mainWindow.window /* OR null */)
                                val result = fileChooser.selectedFile

                                if (result != null) {
                                    if (!result.canRead()) {
                                        JOptionPane.showMessageDialog(
                                            mainWindow.window,
                                            "文件不可读",
                                            "错误",
                                            JOptionPane.ERROR_MESSAGE
                                        )
                                    } else {
                                        Thread {
                                            try {
                                                val newData = OutputHelper(result)
                                                if (!OutputHelper.isMetaDataEquals(helper, newData)) {
                                                    JOptionPane.showMessageDialog(
                                                        mainWindow.window,
                                                        "两文件元数据（前四行）不相同，不能合并，请手动修改。",
                                                        "错误",
                                                        JOptionPane.ERROR_MESSAGE
                                                    )
                                                } else {
                                                    helper.addAll(newData)
                                                    model.setPage(MainViewModel.Page.Welcome)
                                                    JOptionPane.showMessageDialog(
                                                        mainWindow.window,
                                                        "成功，${newData.file.absolutePath} 已经合并到 ${helper.file.absolutePath}。",
                                                        "成功",
                                                        JOptionPane.INFORMATION_MESSAGE
                                                    )
                                                }

                                                System.gc()
                                            } catch (e: Exception) {
                                                JOptionPane.showMessageDialog(
                                                    mainWindow.window,
                                                    "文件解析失败：${e.message}",
                                                    "错误",
                                                    JOptionPane.ERROR_MESSAGE
                                                )
                                            }
                                        }.start()
                                    }
                                }
                            },
                            modifier = Modifier.fillMaxWidth().padding(0.dp, 0.dp, 0.dp, 8.dp)
                        ) {
                            Text("合并数据")
                        }

                        Button(
                            onClick = {
                                Thread {
                                    btnText = "保存中"
                                    enableBtn = false
                                    helper.save()
                                    enableBtn = true
                                    btnText = "保存成功"
                                    Thread.sleep(2000)
                                    btnText = "全部保存"
                                }
                                    .start()
                            },
                            enabled = enableBtn,
                            modifier = Modifier.fillMaxWidth().padding(0.dp, 0.dp, 0.dp, 8.dp)
                        ) {
                            Text(btnText)
                        }
                    }
                }
            }

            // 右侧主面板
            Box(
                modifier = Modifier
                    .weight(0.9f)
                    .fillMaxSize()
                    .padding(8.dp)
            ) {
                when (model.uiState.nowPage) {
                    Add -> AddPage()
                    Manage -> ManagePage()
                    Category -> CategoryPage()
                    Difficulty -> DifficultyPage()
                    Welcome -> WelcomePage()
                    About -> AboutPage()
                }
            }
        }
    }
}

@Composable
fun ButtonClickToPage(targetPage: MainViewModel.Page, str: String) {
    Button(
        onClick = {
            if (model.uiState.nowPage == Category || model.uiState.nowPage == Difficulty) {
                Thread {
                    helper.save()
                }.start()
            }
            model.setPage(targetPage)
        },
        enabled = model.uiState.nowPage != targetPage,
        modifier = Modifier.fillMaxWidth().padding(0.dp, 0.dp, 0.dp, 8.dp)
    ) {
        Text(str)
    }
}

@ExperimentalComposeUiApi
@ExperimentalMaterialApi
fun main(args: Array<String>) {
    UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName())
    when (args.size) {
        0 -> {
            JOptionPane.showMessageDialog(null, "请带上参数")
        }

        1 -> {
            val file = File(args[0])
            if (!file.exists() || file.isDirectory) {
                JOptionPane.showMessageDialog(null, "文件不存在")
            } else if (!file.canWrite()) {
                JOptionPane.showMessageDialog(null, "文件不可写")
            } else if (!file.canRead()) {
                JOptionPane.showMessageDialog(null, "文件不可读")
            } else {
                helper = OutputHelper(file)
                main()
            }
        }

        else -> {
            JOptionPane.showMessageDialog(null, "参数无效")
        }
    }
}

@ExperimentalComposeUiApi
@ExperimentalMaterialApi
fun main() = application {
    Window(onCloseRequest = ::exitApplication, title = "题目编辑器") {
        mainWindow = this
        App()
    }
}

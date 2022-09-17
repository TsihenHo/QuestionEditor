package page

import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.text.selection.SelectionContainer
import androidx.compose.material.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.SpanStyle
import androidx.compose.ui.text.buildAnnotatedString
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextDecoration
import androidx.compose.ui.text.withStyle
import androidx.compose.ui.unit.dp

@Composable
fun AboutPage() {
    SelectionContainer {
        LazyColumn {
            item {
                Text(buildAnnotatedString {
                    append("by 编辑部何子恒\n\n")

                    append("Q: 如何修改难度或者版块的名字？\n")
                    append("A: 关闭软件，打开数据文件，手动修改，")
                    withStyle(SpanStyle(fontWeight = FontWeight.Bold)) {
                        append("但千万不要修改顺序或者数量")
                    }
                    append("。\n\n")

                    append("Q: 怎样多人协祚？\n")
                    append("A: 第一步，由一个人编辑好难度和版块及其权重的内容，将得到的数据文件（除空行外只有四行）分发给其他人，")
                    append("其他使用此数据文件进行编写，")
                    withStyle(SpanStyle(fontWeight = FontWeight.Bold)) {
                        append("编写过程中不能修改版块和难度相关内容")
                    }
                    append("。完成后，使用“合并数据”功能将各个数据文件合并到一起。\n\n")

                    append("Q: 版块/难度类别太多使得添加题目时一个屏幕展示不完？\n")
                    append("A: Shift+鼠标滚轮可以滚动。\n\n")

                    append("Q: 无法选中带有空白符号（如空格、换行）文本？\n")
                    append("A: Jetpack Compose 的锅，实际上是选中了的，只不过看不到，可以正常操作。不信的话你可以在这里试试。\n\n")

                    append("Q: 支持添加图片吗？\n")
                    append("A: 理论上是支持的，使用任何格式都可以，比如像这样：“[[图片路径]]”。为什么？")
                    withStyle(SpanStyle(textDecoration = TextDecoration.LineThrough)) {
                        append("因为我不负责显示内容，显示内容是骆宝负责")
                    }
                    append("。\n\n")

                    append("Q: 制表符显示成这样：\t？\n")
                    append("A: Jetpack Compose 的锅。\n\n")

                    append("Q: 正则表达式是什么？\n")
                    append("A: 一种字符串匹配机制。你可以无视它，当成“包含”就可以了。\n\n")
                }, modifier = Modifier.padding(8.dp))
            }
        }
    }
}
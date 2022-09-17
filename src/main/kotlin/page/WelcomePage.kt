package page

import androidx.compose.foundation.layout.Column
import androidx.compose.material.MaterialTheme
import androidx.compose.material.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.text.style.TextAlign


@Composable
fun WelcomePage() {
    Column {
        Text(
            text = "欢迎",
            style = MaterialTheme.typography.h1,
            textAlign = TextAlign.Center
        )
    }
}
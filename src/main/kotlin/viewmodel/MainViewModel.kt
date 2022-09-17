package viewmodel

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue

class MainViewModel {
    enum class Page {
        Welcome,
        Manage,
        Add,
        Category,
        Difficulty,
        About
    }

    data class MainUiState(
        val nowPage: Page = Page.Welcome
    )

    var uiState: MainUiState by mutableStateOf(MainUiState())
        private set

    fun setPage(page: Page) {
        uiState = uiState.copy(nowPage = page)
    }
}
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', event => {
    const newColorScheme = event.matches ? "theme-dark" : "theme-light";
    document.body.className = newColorScheme
});

if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
    document.body.className = 'theme-dark'
} else {
    document.body.className = 'theme-light'
}

function toggleTheme() {
    if (document.body.className == "theme-dark") {
        document.body.className = 'theme-light'
    } else if (document.body.className = 'theme-light') {
        document.body.className = 'theme-dark'
    } else {
        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            document.body.className = 'theme-dark'
        } else {
            document.body.className = 'theme-light'
        }
    }
}


function hideDropdownClick() {
    var firstEle = document.getElementById("top-dropdown");
    var secondEle = document.getElementById("bot-dropdown");

    firstEle.className = "nav-link dropdown-toggle";
    secondEle.className = "dropdown-menu";
}


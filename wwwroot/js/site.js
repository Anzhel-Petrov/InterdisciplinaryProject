var oKButton = document.getElementById("close");
var removeActive = document.getElementById("searchOpponent");

oKButton.addEventListener('click', function () {
    removeActive.classList.remove('active');
});
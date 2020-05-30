function removeFromFavoriteCategories(id) {
    var elem = $("#cat-" + id);
    elem.css('display', 'none');

    $.get("/User/RemoveFromFavoriteCategory", { id: id });
}

function addNewFavoriteCategory() {
    var value = $('#newFavCat').val();
    $.post("/User/AddFavoriteCategory", { id: +value });
    location.reload();
}

function addToFavoriteProducts(productId) {
    $(".eyeLogo").src = "~/Content/img/heart.svg";

    $.post("/User/AddToFavoriteProduct", { productId: +productId });
}

function delFavCat(id) {
    var elem = $("#" + id);
    elem.css('display', 'none');

    $.get("/User/RemoveFromFavoriteProduct", { id: id });
}
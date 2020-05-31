function removeFromFavoriteCategories(id) {
    var elem = $("#cat-" + id);
    elem.css('display', 'none');

    $.get("/User/RemoveFromFavoriteCategory", { id: id });
    setTimeout(() => location.reload(), 700);
}

function addNewFavoriteCategory() {
    var value = $('#newFavCat').val();
    $.post("/User/AddFavoriteCategory", { id: +value });
    setTimeout(() => location.reload(), 700);
}

function addToFavoriteProducts(productId) {
    $.post("/User/AddToFavoriteProduct", { productId: +productId });
    $(".eyeLogo").attr('src', "../../Content/img/heart.svg");
}

function delFavCat(id) {
    var elem = $("#" + id);
    elem.css('display', 'none');

    $.get("/User/RemoveFromFavoriteProduct", { id: id });
    $(".eyeLogo").attr('src', "../../Content/img/vision1.svg");

    setTimeout(() => location.reload(), 700);
}

function addRemoveFavoriteProduct(productId) {
    var src = $(".eyeLogo").attr("src");
    if (src.indexOf("vision") !== -1) {
        $.post("/User/AddToFavoriteProduct", { productId: +productId });
        $(".eyeLogo").attr('src', "../../Content/img/heart.svg");
    } else {
        $.get("/User/RemoveFromFavoriteProduct", { id: productId });
        $(".eyeLogo").attr('src', "../../Content/img/vision1.svg");
    }
}
function removeFromFavoriteCategories(id) {
    var elem = $("#cat-" + id);
    elem.css('display', 'none');

    $.get("/User/RemoveFromFavoriteCategory", { id: id });
}

function addNewFavoriteCategory() {
    var value = $('#newFavCat').val();
    $.post("/User/AddFavoriteCategory", { id: value });
    location.reload();
}
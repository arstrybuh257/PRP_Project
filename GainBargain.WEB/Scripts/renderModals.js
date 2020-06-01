$(document).ready(function () {
    $.ajaxSetup({ cache: false });

    $('#registerButton').click(function (e) {
        e.preventDefault();
        $.get(this.href, function (data) {
            $("#registerModal").html(data);
            $("#registerModal").modal('show');
        });
    });

    $('#loginButton').click(function (e) {
        e.preventDefault();
        $.get(this.href, function (data) {
            $("#loginModal").html(data);
            $("#loginModal").modal('show');
        });
    });
});

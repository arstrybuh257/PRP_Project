$().ready(function () {
    $.validator.addMethod("email", function (value, element) {
        return this.optional(element) || /^[a-zA-Z0-9._-]+@[a-zA-Z0-9-]+\.[a-zA-Z.]{2,5}$/i.test(value);
    }, "Введіть правильну електронну пошту");

    $.validator.addMethod("password", function (value, element) {
        return this.optional(element) || /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,16}$/i.test(value);
    }, "Пароль повинен бути довжиною 8-16 символів, мати як великі, так і маленькі літери, а також числа");


    $("#registerForm").validate({
        rules: {
            Email: "required email",
            Password: "required password"
        },
    });
});
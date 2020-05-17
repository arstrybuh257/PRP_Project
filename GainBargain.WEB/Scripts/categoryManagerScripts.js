if (window.$) {
    function showSubCategories(catId, butId) {
        let categories = document.getElementById(catId);
        let displayCats = categories.style.display;
        categories.style.display = displayCats == 'none' ? 'block' : 'none';

        let butt = document.getElementById(butId);
        let visibButt = butt.style.visibility;
        butt.style.visibility = visibButt == 'hidden' ? 'visible' : 'hidden';
    }

    function showCategoryForm(id) {
        let form = document.getElementById(id);
        let visibility = form.style.visibility;
        form.style.visibility = visibility == 'hidden' ? 'visible' : 'hidden';
    }

    function refreshCategories(id, textBoxID,formId) {
        let text = document.getElementById(textBoxID).value;
        document.getElementById(textBoxID).value = "";
        document.getElementById(formId).style.visibility = 'hidden';

        $("#" + id).load('/Admin/SubCategoriesPartial?id=' + id,'text='+text); 
    }

    function editCategory(categId, supCategId) {
        //id, name, submitButton
        //$("#my_id .my_class")
        let formId = '#editForm-' + categId;
        //let categName = $(formId + " .categName");
        //$("#" + supCategId).load('/Admin/SubCategoriesPartial?id=' + categId, 'name=' + categName, 'submitButton=edit',
        //    function (response, status, xhr) {
        //       alert(response);
        //    });

        $(formId).submit();
        $("#" + supCategId).load('/Admin/SubCategoriesPartial?id=' + supCategId); 


        //$("#" + supCategId).load('/Admin/SubCategoriesPartial',
        //    {
        //        id: categId,
        //        name: categName,
        //        submitButton: "edit"
        //    },
        //    function (response, status, xhr) {
        //        alert(response);
        //    });
    }
}
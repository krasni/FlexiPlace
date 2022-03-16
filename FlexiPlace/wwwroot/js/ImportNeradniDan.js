$().ready(function () {
    $('#form1').submit(function (evt) {
        if ($('#form1').valid()) {
            uploadFile();
            evt.preventDefault();
        }
        else {
            evt.preventDefault();
        }
    });

    $('.custom-file-input').on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).next('.custom-file-label').html(fileName);
    })
});

function uploadFile() {
    var formData = new FormData();

    var file = document.getElementById('ExcelNeradniDani').files[0];
    formData.append("formFile", file);

    $.ajax({
        url: "/importNeradniDani",
        type: "POST",
        data: formData,
        processData: false,  // tell jQuery not to process the data
        contentType: false,   // tell jQuery not to set contentType

        success: function (results) {
            if (results.code === 0) {
                alert('Datoteka je uspješno importirana.');
            }
            else {
                alert('Došlo je do greške prilikom importa: ' + results.msg);
            }
        },

        error: function (xhr, ajaxOptions, thrownError) {
            alert('Dogodila se greška kod import datuma za praznike.');
        }
    });
}

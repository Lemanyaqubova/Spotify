$(function () {
    $(document).on("click", ".btn-danger", function (e) {
        e.preventDefault();
        let imageId = $(this).attr("product-id");
        let deletedElement = $(this).parent();
        let data = { id: imageId };

        $.ajax({
            url: "/AdminArea/Product/DeleteProductImage",
            type: "Post",
            data: data,
            success: function (res) {
                if (res) {
                    $(deletedElement).remove();
                } else {
                    alert("Product images must be min 1")
                }

            }

        })
    })
})
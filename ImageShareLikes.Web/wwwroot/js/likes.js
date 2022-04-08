$(() => {
    $("#like-button").on("click", function () {
        const id = $("#image-id").val();
        const liked = true;
        $.post("/home/viewimage", { id, liked }, function () {
            updateLikes();
            $("#like-button").prop('disabled', true);
        });
    });

    function updateLikes() {
        const id = $("#image-id").val();
        $.get("/home/getlikes", { id }, function (result) {
            $("#likes-count").text(result);
        })
    }

    setInterval(() => {
        updateLikes();
    }, 1000)
});
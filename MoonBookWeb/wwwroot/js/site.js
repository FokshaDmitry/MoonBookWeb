let search = document.getElementById("search_user");
let searchText = document.getElementById("search_user_text");

search.addEventListener("click", () => {
    if (searchText.value != "") {
        fetch("/api/freand", {
            method: "PUT",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            body: `Name=${searchText.value}`
        }).then(r => r.json()).then(j => {
            if (j.status == "Ok") {
                alert(j.message[0].name + " " + j.message[0].surname)
            }
            else if (j.status == "Undefinded") {
                alert(j.message)
            }
            else {
                alert(j.message);

            }
        })
    }
    else {
        location.reload();
    }
})
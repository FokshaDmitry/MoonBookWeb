document.addEventListener("DOMContentLoaded", () => {
    const freand = document.querySelector("freands");
    if (!freand) throw "Forum  script: APP not found";
    loadFreands(freand);
});
let search = document.getElementById("search_user");
let searchText = document.getElementById("search_user_text");
let freand;
let addFreand = document.getElementById("AddFreand")

function loadFreands(elem) {
    freand = elem;
    fetch("/api/freand",
        {
            method: "GET",
            headers: {
                "User-Id": "",
                "Culture": ""
            },
            body: null
        })
        .then(r => r.json())
        .then(j => {
            if (j instanceof Array) {
                showUserFreands(elem, j);
            }
            else {
                throw "showTopics: Backend data invalid";
            }
        });
}
function showUserFreands(elem, j) {
    fetch("/tmpl/freands.html")
        .then(r => r.text())
        .then(trTemplate => {
            var appHtml = "";
            for (let freand of j) {
                var tmp = trTemplate
                let b = false;
                tmp = tmp
                    .replace("{{id}},", freand.id)
                    .replace("{{Name}}", freand.name)
                    .replace("{{Surname}}", freand.surname)
                    .replace("{{SrcIcon}}", "person_remove_FILL0_wght400_GRAD0_opsz48.png")
                if (freand.photoName != null) {
                    tmp = tmp.replace("{{PhotoName}}", freand.photoName)
                } else {
                    tmp = tmp.replace("{{PhotoName}}", "android_contacts_FILL0_wght400_GRAD0_opsz48.png");
                }
                appHtml += tmp;
            }
            elem.innerHTML = appHtml;
        });
}
function searchFreands() {
    if (searchText.value != "") {
        fetch("/api/freand", {
            method: "PUT",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            body: `Name=${searchText.value}`
        }).then(r => r.json())
            .then(j => {
            if (j.status == "Ok") {
                showFreands(freand, j.message, j.sub)
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
}
function showFreands(elem, j, sub) {
    fetch("/tmpl/freands.html")
        .then(r => r.text())
        .then(trTemplate => {
            var appHtml = "";
            for (let freand of j) {
                var tmp = trTemplate
                let b = false;
                tmp = tmp
                    .replace("{{id}},", freand.id)
                    .replace("{{Name}}", freand.name)
                    .replace("{{Surname}}", freand.surname)
                if (freand.photoName != null) {
                    tmp = tmp.replace("{{PhotoName}}", freand.photoName)
                } else {
                    tmp = tmp.replace("{{PhotoName}}", "android_contacts_FILL0_wght400_GRAD0_opsz48.png");
                }
                if (sub.indexOf(freand.id) != -1) {
                    tmp = tmp.replace("{{SrcIcon}}", "person_remove_FILL0_wght400_GRAD0_opsz48.png")
                }
                else {
                    tmp = tmp.replace("{{SrcIcon}}", "person_add_FILL0_wght400_GRAD0_opsz48.png")
                }
                appHtml += tmp;
            }
            elem.innerHTML = appHtml;
        });
}
search.addEventListener("click", searchFreands)

let freands;
let posts
document.addEventListener("DOMContentLoaded", () => {
    const freand = document.querySelector("freands");
    if (!freand) throw "Forum  script: APP not found";
    freands = new Freands(freand, "/api/freand")
    freands.loadFreands();
    const post = document.querySelector("post");
    if (!post) throw "Forum  script: APP not found";
    posts = Posts(post, "/api/freand");
    posts.loadElement("Post")
});
let search = document.getElementById("search_user");

class Freands
{
    constructor(elem, API) {
        this.elem = elem;
        this.API = API;
    }
    
    loadFreands() {
        fetch(this.API,
        {
            method: "GET",
            body: null
        })
        .then(r => r.json())
        .then(j => {
            if (j.message instanceof Array) {
                this.showUserFreands(this.elem, j.message);
            }
            else {
                throw "showTopics: Backend data invalid";
            }
        });
    }

    showUserFreands(elem, j) {
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
                this.freandLoaded();
            });
    }
    async freandLoaded() {
        for (let freand of document.querySelectorAll(".idFreand")) {
            freand.onclick = this.Follow;
        }
    }
    Follow(e) {
        let follow = e.currentTarget.childNodes[5].childNodes[1];
        let idFreand = e.currentTarget.getAttribute("id")
        if (e.target === follow) {
            fetch(`/api/freand/${idFreand}`, {
                method: "PUT",
                body: null
            }).then(r => r.json()).then(j => {
                if (j.status == "Error") {
                    alert(j.message)
                } else {

                }
            })
        }
        if (e.target !== follow) {
            const formData = new FormData();
            formData.append("Reaction", 1);
            formData.append("IdPost", idPost);
            fetch(`/api/freand/${idFreand}`, {
                method: "PUT",
                body: null
            }).then(r => r.json()).then(j => {
                if (j.status == "Error") {
                    alert(j.message)
                } else {
    
                }
            })
        }
    }
    searchFreands(searchText) {
        if (searchText.value != "") {
            fetch(this.API, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                body: `Name=${searchText.value}`
            }).then(r => r.json())
                .then(j => {
                    if (j.status == "Ok") {
                        this.showFreands(this.elem, j.message, j.sub)
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
    showFreands(elem, j, sub) {
        fetch("/tmpl/freands.html")
            .then(r => r.text())
            .then(trTemplate => {
                var appHtml = "";
                for (let freand of j) {
                    var tmp = trTemplate
                    let b = false;
                    tmp = tmp
                        .replace("{{id}}", freand.id)
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
                this.freandLoaded()
            });
    }
}

search.addEventListener("click", () => {
    freands.searchFreands(document.getElementById("search_user_text"))
})
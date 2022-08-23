
let posts
let post
document.addEventListener("DOMContentLoaded", () => {
    const freand = document.querySelector("freands");
	if (!freand) throw "Forum  script: APP not found";
	post = document.querySelector("post");
	loadFreandElements(freand)
	if (!post) throw "Forum  script: APP not found";
	posts = new Posts("/api/freand/Post");
	posts.loadElement(post);
});
let search = document.getElementById("search_user");
search.addEventListener("click", () => {
    freands.searchFreands(document.getElementById("search_user_text"))
})

function loadFreandElements(elem) {
	fetch(`/api/freand`,
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
			if (j.status === "Ok") {
				showElement(elem, j.message);
			}
			else {
				throw "showTopics: Backend data invalid";
			}
		});
}
function showElement(elem, j) {
	fetch("/tmpl/freands.html")
		.then(r => r.text())
		.then(trTemplate => {
			let appHtml = "";
			for (let freand of j) {
				var tmp = trTemplate
				tmp = tmp.replaceAll("{{id}}", freand.id)
					.replaceAll("{{Name}}", freand.name)
					.replaceAll("{{Surname}}", freand.surname)
					.replaceAll("{{SrcIcon}}", "person_remove_FILL0_wght400_GRAD0_opsz48.png")
					.replaceAll("{{PhotoName}}", (freand.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : freand.photoName))
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
			this.freandLoaded();
		});
}
function searchFreands(searchText) {
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
					this.showUserFreands(this.elem, j.message)
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
async function freandLoaded() {
	for (let freand of document.querySelectorAll(".idFreand")) {
		freand.onclick = this.Follow;
	}
}
function Follow(e) {
	let follow = e.currentTarget.childNodes[5].childNodes[1];
	let idFreand = e.currentTarget.getAttribute("id")
	if (e.target === follow) {
		fetch(`/api/freand/${idFreand}`, {
			method: "PUT",
			body: null
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				alert(j.message)
			} else if (j.status = "Ok") {
				if (j.message) {
					follow.src = "/icons/person_remove_FILL0_wght400_GRAD0_opsz48.png";
				} else {
					follow.src = "/icons/person_add_FILL0_wght400_GRAD0_opsz48.png";
				}
			}
		})
	}
	if (e.target !== follow) {
		fetch(`/api/freand/${idFreand}`, {
			method: "GET",
			body: null
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				alert(j.message)
			} else {
				var appHtml = `<div id="FreandPageInfo"><img id="FreandPagePhoto" src = "/img/{{PhotoName}}"> <p id="FreandPageNeme"><b>{{Name}} {{Surname}}</b></p></div>`;
				for (let freand of j.message) {
					appHtml = appHtml
						.replace("{{Name}}", freand.user.name)
						.replace("{{Surname}}", freand.user.surname)
						.replace("{{PhotoName}}", (freand.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : freand.user.photoName))
				}
				posts.showElement(post, j.message, appHtml);
			}
		})
	}
}
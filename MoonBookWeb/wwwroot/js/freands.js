let posts
let post
let freand
var link
document.addEventListener("DOMContentLoaded", () => {
    freand = document.querySelector("freands");
	if (!freand) throw "Forum  script: APP not found";
	post = document.querySelector("post");
	loadFreandElements(freand)
	if (!post) throw "Forum  script: APP not found";
	posts = new Posts("/api/freand/Post");
	posts.loadElement(post);
	link = new URL(window.location.href);
	
});
let search = document.getElementById("search_user");


function loadFreandElements(elem) {
	fetch(`/api/freand`,
		{
			method: "GET",
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
				tmp = tmp.replaceAll("{{id}}", freand.user.id)
					.replaceAll("{{Name}}", freand.user.name)
					.replaceAll("{{Surname}}", freand.user.surname)
					.replaceAll("{{SrcIcon}}", (freand.sub.length == 0 ? "person_add_FILL0_wght400_GRAD0_opsz48.png" : "person_remove_FILL0_wght400_GRAD0_opsz48.png"))
					.replaceAll("{{PhotoName}}", (freand.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : freand.user.photoName))
					.replaceAll("{{Online}}", (freand.user.online ? "" : "background-color: lightpink;"))
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
			freandLoaded();
		});
}
function searchFreands(searchText) {
	if (searchText.value != "") {
		fetch(`/api/freand/${searchText.value}`, {
			method: "PUT",
			body: null
        }).then(r => r.json()).then(j => {
				if (j.status == "Ok") {
					showElement(freand, j.message)
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
		freand.onclick = Follow;
	}
	link = link.search.replace('?', "");
	if (link != '') {
		fetch(`/api/freand/${link}`, {
			method: "GET",
			body: null
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				alert(j.message)
			} else {
				var appHtml = `<div id="FreandPageInfo"><img id="FreandPagePhoto" src = "/img/{{PhotoName}}"> <p id="FreandPageNeme"><b>{{Name}} {{Surname}}</b></p></div>`;
				appHtml = appHtml
					.replace("{{Name}}", j.user.name)
					.replace("{{Surname}}", j.user.surname)
					.replace("{{PhotoName}}", (j.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : j.user.photoName))
				posts.showElement(post, j.freandsPost, appHtml);
			}
		})
	}
}
function Follow(e) {
	let follow = e.currentTarget.childNodes[7].childNodes[1];
	let idFreand = e.currentTarget.getAttribute("id")
	if (e.target === follow) {
		fetch(`/api/freand/${idFreand}`, {
			method: "POST",
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
				appHtml = appHtml
					.replace("{{Name}}", j.user.name)
					.replace("{{Surname}}", j.user.surname)
					.replace("{{PhotoName}}", (j.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : j.user.photoName))
				posts.showElement(post, j.freandsPost, appHtml);
			}
		})
	}
}
search.addEventListener("click", () => {
	searchFreands(document.getElementById("search_user_text"))
})
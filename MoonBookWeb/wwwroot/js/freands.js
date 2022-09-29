let posts
let post
let freand
var link
document.addEventListener("DOMContentLoaded", () => {
	//element <freands><freands/>
    freand = document.querySelector("freands");
	if (!freand) throw "Forum  script: APP not found";
	loadFreandElements(freand)
	//element <post><post/>
	post = document.querySelector("post");
	if (!post) throw "Forum  script: APP not found";
	post.innerHTML = `<img style="width: 150px; position: absolute; margin-left: 20%;" src ="../icons/oie_L36bLHLNsDH2.gif"/>`
	posts = new Posts("/api/freand/Post");
	posts.loadElement(post);
	//get path
	link = new URL(window.location.href);
});
let search = document.getElementById("search_user");
//load user freands
function loadFreandElements(elem) {
	fetch(`/api/freand`,
		{
			method: "GET",
			body: null
		})
		.then(r => r.json())
		.then(j => {
			if (j.status === "Ok") {
				showElementFreand(elem, j.message);
			}
			else {
				throw "showTopics: Backend data invalid";
			}
		});
}
function showElementFreand(elem, j) {
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
//Serch freands for name
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
// add event
async function freandLoaded() {
	for (let freand of document.querySelectorAll(".idFreand")) {
		freand.onclick = Follow;
	}
	//parse link
	link = link.search.replace('?', "");
	//if link not empty, show choose user
	if (link != '') {
		PageFreand(link);
	}
}
function Follow(e) {
	let follow = e.currentTarget.childNodes[7].childNodes[1];
	let idFreand = e.currentTarget.getAttribute("id")
	//follow/unfollow
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
	//Show freand page
	if (e.target !== follow) {
		PageFreand(idFreand)
	}
}
function PageFreand(idFreand) {
	fetch(`/api/freand/${idFreand}`, {
		method: "GET",
		body: null
	}).then(r => r.json()).then(j => {
		if (j.status == "Error") {
			alert(j.message)
		} else {
			//Fread freands
			var freandsHtml = `<div class="Freand" style="display: flex; margin: 1%; justify-content: flex-end; align-items: flex-end;" title="{{Name}} {{Surname}}" id="{{id}}">
									<img id="FreandFreandsPhoto" src="/img/{{UserPhoto}}"/>
									<div id="OnlineUser" style="position: absolute; {{Online}}"></div>
								</div>`
			//freand book
			var bookHtml = `<div class="Book" title="{{Author}} &ldquo;{{Title}}&rdquo;" id="{{id}}">
									<img id="UserBookImg" src="/img_post/{{CoverName}}"/>
							</div>`;
			//Freand info
			var appHtml = `<div id="FreandPageInfo"> 
									<img id="BackFreandsBlog" src = "/icons/arrow_back_FILL0_wght400_GRAD0_opsz48.png">
									<img id="FreandPagePhoto" src = "/img/{{PhotoName}}">
									<p id="FreandPageNeme"><b>{{Name}} {{Surname}}</b></p>
									{{Freands}}
									{{Books}}
							   </div>`;
			var tmpfreand = "";
			if (j.freandFreands !== null && j.freandFreands !== undefined && j.freandFreands !== "" && j.freandFreands.length > 0) {
				for (let freand of j.freandFreands) {
					tmpfreand += freandsHtml
						.replaceAll("{{id}}", freand.id)
						.replaceAll("{{UserPhoto}}", (freand.photoName == null || freand.photoName == "" ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : freand.photoName))
						.replaceAll("{{Name}}", freand.name)
						.replaceAll("{{Surname}}", freand.surname)
						.replaceAll("{{Online}}", (freand.online ? "" : "background-color: lightpink;"))
				}
            }
			var tmpbook = "";
			if (j.book !== null && j.book !== undefined && j.book !== "" && j.book.length > 0) {
				for (let book of j.book) {
					tmpbook += bookHtml
						.replaceAll("{{id}}", book.id)
						.replaceAll("{{CoverName}}", (book.coverName == null || book.coverName == "" ? "local_library_FILL0_wght500_GRAD0_opsz48.png" : book.coverName))
						.replaceAll("{{Author}}", book.author)
						.replaceAll("{{Title}}", book.title)
				}
            }
			
			appHtml = appHtml
				.replace("{{Name}}", j.freand.name)
				.replace("{{Surname}}", j.freand.surname)
				.replace("{{PhotoName}}", (j.freand.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : j.freand.photoName))
				.replace("{{Freands}}", (tmpfreand == "" ? "" : `<div> <p id="TitleFreand"><b>Freands</b></p> <freand id= "FreandFreands"> ${tmpfreand} </freand></div>`))
				.replace("{{Books}}", (tmpbook == "" ? "" : `<books id="UserBooks"> ${tmpbook} </books>`))
			//Freand posts
			posts.showElement(post, j, appHtml, bookLoaded);
		}
	})
}
// Add event book 
async function bookLoaded() {
	for (let book of document.querySelectorAll(".Book")) {
		book.onclick = Read;
	}
	freabdsLoaded();
	backLoaded();
}
async function backLoaded() {
	document.querySelector("#BackFreandsBlog").onclick = function () {
		window.location.href = `/User/FreandPage`;
    };
}

// Add event Freand freands 
async function freabdsLoaded() {
	for (let freand of document.querySelectorAll(".Freand")) {
		freand.onclick = FreandFreands;
	}
}
//Redirect on My Library 
function FreandFreands(e) {
	let idFreand = e.currentTarget.getAttribute("id")
	window.location.href = `/User/FreandPage?${idFreand}`;
}
//Redirect on My Library 
function Read(e) {
	let idBook = e.currentTarget.getAttribute("id")
	window.location.href = `/Books/UserLibrary?${idBook}`;
}
//search fread
search.addEventListener("click", () => {
						//Name freand
	searchFreands(document.getElementById("search_user_text"))
})
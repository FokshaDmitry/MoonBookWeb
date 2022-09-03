var Title = document.getElementById("Title");
var Text = document.getElementById("Text");
var TextComment = document.getElementById("TextComment");
var Post = document.getElementById("Post");
var ImagePost = document.getElementById("ImagePost");
var valTitle = "";
document.addEventListener("DOMContentLoaded", () => {
	const post = document.querySelector("post");
	if (!post) throw "Forum  script: APP not found";
	var posts = new Posts("/api/post");
	posts.loadElement(post);
	const online = document.querySelector("online");
	if (!online) throw "Forum  script: APP not found";
	loadElement(online);
	const books = document.querySelector("books");
	if (!books) throw "Forum  script: APP not found";
	loadElementBook(books);
});

Title.addEventListener("click", () => {
	var start = Text.selectionStart;
	var end = Text.selectionEnd;
	valTitle = Text.value.substring(start, end);
	Text.value = Text.value.substr(0, start) + ">" + Text.value.substr(start, end) + "<" + Text.value.substr(end);
	Text.setSelectionRange(end, end);
})
Post.addEventListener("click", () => {
	const formData = new FormData();
	formData.append("TitlePost", valTitle);
	formData.append("TextPost", Text.value);
	formData.append("ImagePost", ImagePost.files[0]);
	fetch("/API/Post", {
		method: "POST",
		body: formData
	}).then(r => r.json()).then(j => {
		if (j.status == "Error") {
			alert(j.message)
		} else {
			location.reload();
		}
	})
});

function loadElementBook(elem) {
	fetch(`/api/user/Books`,
		{
			method: "GET",
			body: null
		})
		.then(r => r.json())
		.then(j => {
			if (j.status === "Ok") {
				showElementBooks(elem, j.message);
			}
			else {
				throw "showTopics: Backend data invalid";
			}
		});
}
function loadElement(elem) {
	fetch(`/api/user`,
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
function showElementBooks(elem, j) {
	fetch("/tmpl/userbooks.html")
		.then(r => r.text())
		.then(trTemplate => {
			let appHtml = "";
			for (let book of j) {
				var tmp = trTemplate
				tmp = tmp.replaceAll("{{id}}", book.id)
					.replaceAll("{{CoverName}}", (book.coverName == null || book.coverName == "" ? "local_library_FILL0_wght500_GRAD0_opsz48.png" : book.coverName))
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
		});
}
function showElement(elem, j) {
	fetch("/tmpl/onlineFrands.html")
		.then(r => r.text())
		.then(trTemplate => {
			let appHtml = "";
			for (let freand of j) {
				var tmp = trTemplate
				tmp = tmp.replaceAll("{{id}}", freand.id)
					.replaceAll("{{PhotoName}}", (freand.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : freand.photoName))
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
			freandLoaded();
		});
}
async function freandLoaded() {
	for (let freand of document.querySelectorAll(".idOnlineFreand")) {
		freand.onclick = View;
	}
}
function View(e) {
	let idFreand = e.currentTarget.getAttribute("id")
	window.location.href = `/User/FreandPage?${idFreand}`;
}
var Title = document.getElementById("Title");
var Text = document.getElementById("Text");
var TextComment = document.getElementById("TextComment");
var Post = document.getElementById("Post");
var ImagePost = document.getElementById("ImagePost");
var valTitle = "";
document.addEventListener("DOMContentLoaded", () => {
	const post = document.querySelector("post");
	if (!post) throw "Forum  script: APP not found";
	post.innerHTML = `<img style="width: 150px; position: absolute; margin-left: 20%;" src ="../icons/oie_L36bLHLNsDH2.gif"/>`
	//Add current user post
	var posts = new Posts("/api/post");
	posts.loadElement(post);
	//online Freands
	const online = document.querySelector("online");
	if (!online) throw "Forum  script: APP not found";
	loadElementFreand(online);
	//User Books
	const books = document.querySelector("books");
	if (!books) throw "Forum  script: APP not found";
	loadElementBook(books);
});
//Title selecton
Title.addEventListener("click", () => {
	var start = Text.selectionStart;
	var end = Text.selectionEnd;
	valTitle = Text.value.substring(start, end);
	Text.value = Text.value.substr(0, start) + ">" + Text.value.substr(start, end) + "<" + Text.value.substr(end);
	Text.setSelectionRange(end, end);
})
// Add new post current User
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
//Load books  elem: <books><books/>
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
//Load online freands elem: <freand><freand/>
function loadElementFreand(elem) {
	fetch(`/api/user`,
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
//Show User Books
function showElementBooks(elem, j) {
	fetch("/tmpl/userbooks.html")
		.then(r => r.text())
		.then(trTemplate => {
			let appHtml = "";
			for (let book of j) {
				var tmp = trTemplate
				tmp = tmp.replaceAll("{{id}}", book.id)
					.replaceAll("{{CoverName}}", (book.coverName == null || book.coverName == "" ? "local_library_FILL0_wght500_GRAD0_opsz48.png" : book.coverName))
					.replaceAll("{{Author}}", book.author)
					.replaceAll("{{Title}}", book.title)
				appHtml += tmp;
			}
			elem.innerHTML += appHtml;
			bookLoaded();
		});
}
//Show Online freands
function showElementFreand(elem, j) {
	let appHtml = "";
	for (let freand of j) {
		var tmp = `<div class="idOnlineFreand" title="{{Name}} {{Surname}}">
						<a href="../User/FreandPage?{{id}}"> <img id="OnlineFreandsPhoto" src="../img/{{PhotoName}}"/> </a>
				   </div>`
		tmp = tmp.replaceAll("{{id}}", freand.login)
			.replaceAll("{{Name}}", freand.name)
			.replaceAll("{{Surname}}", freand.surname)
			.replaceAll("{{PhotoName}}", (freand.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : freand.photoName))
		appHtml += tmp;
	}
	elem.innerHTML = appHtml;

}
// Add event book 
async function bookLoaded() {
	for (let book of document.querySelectorAll(".Book")) {
		book.onclick = Read;
	}
}
//Redirect on My Library 
function Read(e) {
	let idBook = e.currentTarget.getAttribute("id")
	window.location.href = `/Books/UserLibrary?${idBook}`;
}
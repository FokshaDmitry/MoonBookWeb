let link;
document.addEventListener("DOMContentLoaded", () => {
	const userbook = document.getElementById("UserBook");
	if (!userbook) throw "Forum  script: APP not found";
	loadElement(userbook)
	link = new URL(window.location.href);

});
let idReadBook;
let addbook = document.querySelector("#AddBookLibrary");
addbook.addEventListener("click", () => {

})
function loadElement(elem) {
	fetch(`/api/book`,
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
function showElementBooks(elem, j) {
	fetch("/tmpl/bookitem.html")
		.then(r => r.text())
		.then(trTemplate => {
			let appHtml = "";
			for (let book of j) {
				var tmp = trTemplate
				tmp = tmp.replaceAll("{{id}}", book.id)
					.replaceAll("{{Title}}", book.title)
					.replaceAll("{{Author}}", book.author)
					.replaceAll("{{CoverName}}", (book.coverName == null || book.coverName == "" ? "import_contacts_FILL0_wght400_GRAD0_opsz48.png" : book.coverName))
				appHtml += tmp;
			}
			elem.innerHTML = appHtml;
			bookLoaded();
		});
}
async function bookLoaded() {
	for (let book of document.querySelectorAll(".BookItem")) {
		book.onclick = Read;
	}
	link = link.search.replace('?', "");
	//if link not empty, show choose user
	if (link != '') {
		ReadBook(link);
	}
}
function Read(e) {
	let idBook = e.currentTarget.getAttribute("id");
	ReadBook(idBook)
}
function ReadBook(idBook) {
	fetch(`/api/book/${idBook}`, {
		method: "GET",
		body: null
	}).then(r => r.json()).then(j => {
		if (j.status == "Error") {
			throw `Book.ReadBook: ${j.status}`;
		} else {
			idReadBook = j.message.id;
			let TextContent = document.getElementById("TextContent");
			let appHtml = "";
			addbook.removeAttribute('hidden')
			let addbookimg = document.getElementById("AddBookLibraryImg")
			for (let p of j.message.textContent.split("\n")) {

				appHtml += `<p>${p}</p>`;
			}
			TextContent.innerHTML = appHtml
			if (j.follow) {
				addbookimg.src = "../icons/bookmark_remove_FILL0_wght400_GRAD0_opsz48.png"
			}
			else {
				addbookimg.src = "../icons/bookmark_add_FILL0_wght400_GRAD0_opsz48.png"
            }
		}
	})
}
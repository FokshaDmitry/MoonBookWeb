let link;
document.addEventListener("DOMContentLoaded", () => {
	//User Books
	const userbook = document.getElementById("UserBook");
	if (!userbook) throw "Forum  script: APP not found";
	loadElement(userbook, "GET")
	//Books in User Library
	const librarybook = document.getElementById("LibraryUser");
	if (!librarybook) throw "Forum  script: APP not found";
	loadElement(librarybook, "PUT")
	link = new URL(window.location.href);
	loadPosition();

});
let idReadBook;
let addbook = document.querySelector("#AddBookLibrary");
let size = document.getElementById("size");
let BackgrountColor = document.getElementById("BackgrountColor");
let TextColor = document.getElementById("TextColor");
let Interval = document.getElementById("Interval");

Interval.addEventListener("change", () => {
	document.querySelector("#TextContent").style.lineHeight = `${Interval.value}`;
})

BackgrountColor.addEventListener("change", () => {
	let textContent = document.querySelector("#BackgrountColorText")
	textContent.style.backgroundColor = BackgrountColor.value;
})

TextColor.addEventListener("change", () => {
	for (let color of document.querySelectorAll("#TextContent p")){
		color.style.color = TextColor.value;
    }
})

size.addEventListener("change", () => {
	document.querySelector("#TextContent").style.fontSize = `${size.value}px`;
})

async function loadPosition() {
	for (let radio of document.querySelectorAll("#RadioButtonGroup input[type=radio]")) {
		radio.onclick = Position;
	}
}
//Text Orintations
function Position(e) {
	if (e.target.value == 1) {
		document.querySelector("#TextContent").style.textAlign = `center`;
	}
	if (e.target.value == 2) {
		document.querySelector("#TextContent").style.textAlign = `right`;
	}
	if (e.target.value == 3) {
		document.querySelector("#TextContent").style.textAlign = `left`;
	}
	if (e.target.value == 4) {
		document.querySelector("#TextContent").style.textAlign = `justify`;
	}
}

addbook.addEventListener("click", () => {
	fetch(`/api/book/${idReadBook}`,
		{
			method: "POST",
			body: null
		})
		.then(r => r.json())
		.then(j => {
			if (j.status === "Error") {
				throw `addBook: ${j.status}`;
			}
			else {
				location.reload();
			}
		});
})

function loadElement(elem, Query) {
	fetch(`/api/book`,
		{
			method: Query,
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
function bookLoaded() {
	for (let book of document.querySelectorAll(".BookItem")) {
		book.onclick = Read;
	}
	if (link !== "") {
		link = link.search.replace('?', "");
		//if link not empty, show choose user
		if (link !== '') {
			ReadBook(link);
			link = "";
		}
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
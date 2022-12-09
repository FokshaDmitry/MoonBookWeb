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
let textContent = document.getElementById("TextContent");
let textComment = document.getElementById("TextComment");
let send = document.getElementById("SendCommentImg");
let quote = "";
let dialog;
let searchP;
//Send Quote
send.addEventListener("click", () => {
	if (textComment !== "" && quote !== "" && link !== "") {
		const formData = new FormData();
		formData.append("Id", link);
		formData.append("Quote", quote);
		formData.append("Text", textComment.textContent);
		formData.append("Search", searchP);
		fetch(`/api/comment/book`, {
			method: "POST",
			body: formData
		}).then(r => r.json())
			.then(j => {
				if (j.status === "Error") {
					alert(`Comment.Quote: ${j.message}`)
				} else {
					dialog.style.visibility = "hidden";
					quote = ""
				}
			});
    }
})
//Set position <p> select element
textContent.addEventListener("mousedown", (e) => {
	let massP = document.querySelectorAll("#TextContent p")
	for (var i = 0; i < massP.length; i++) {
		if (massP[i] === e.target) {
			searchP = i;
        }
    }
})
//Show quoting dialog
textContent.addEventListener("mouseup", (e) => {
	quote = window.getSelection().toString();
	dialog = document.getElementById("Dialog")
	if (quote !== "") {
		dialog.style.visibility = "visible";
		dialog.style.top = `${e.layerY}px`
		dialog.style.left = `${e.layerX}px`
		dialog.querySelector("#TextComment").focus();
	}
	else {
		dialog.style.visibility = "hidden";
		quote = ""
    }
})
//Text settings
Interval.addEventListener("change", () => {
	textContent.style.lineHeight = `${Interval.value}`;
})

BackgrountColor.addEventListener("change", () => {
	let textBackgrount = document.querySelector("#BackgrountColorText")
	textBackgrount.style.backgroundColor = BackgrountColor.value;
})

TextColor.addEventListener("change", () => {
	for (let color of document.querySelectorAll("#TextContent p")){
		color.style.color = TextColor.value;
    }
})

size.addEventListener("change", () => {
	textContent.style.fontSize = `${size.value}px`;
})

async function loadPosition() {
	for (let radio of document.querySelectorAll("#RadioButtonGroup input[type=radio]")) {
		radio.onclick = Position;
	}
}
//Text Orintations
function Position(e) {
	if (e.target.value == 1) {
		textContent.style.textAlign = `center`;
	}
	if (e.target.value == 2) {
		textContent.style.textAlign = `right`;
	}
	if (e.target.value == 3) {
		textContent.style.textAlign = `left`;
	}
	if (e.target.value == 4) {
		textContent.style.textAlign = `justify`;
	}
}
//Add book to user library
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
//Show books
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
		let linkurl = link.search.replace('?', "").split('&');
		//if link not empty, show choose user
		ReadBook(linkurl[0], link.searchParams.get('search'));
		link = "";
    }
}
//Add event
function Read(e) {
	let idBook = e.currentTarget.getAttribute("id");
	ReadBook(idBook)
}
//Choose book
function ReadBook(idBook, search) {
	fetch(`/api/book/${idBook}`, {
		method: "GET",
		body: null
	}).then(r => r.json()).then(j => {
		if (j.status == "Error") {
			throw `Book.ReadBook: ${j.status}`;
		} else {
			idReadBook = j.message.id;
			let appHtml = "";
			addbook.removeAttribute('hidden')
			let addbookimg = document.getElementById("AddBookLibraryImg")
			for (let p of j.message.textContent.split("\n")) {

				appHtml += `<p>${p}</p>`;
			}
			textContent.innerHTML = appHtml
			if (j.follow) {
				addbookimg.src = "../icons/bookmark_remove_FILL0_wght400_GRAD0_opsz48.png"
			}
			else {
				addbookimg.src = "../icons/bookmark_add_FILL0_wght400_GRAD0_opsz48.png"
			}
			link = j.message.id;
			//Quote position
			if (search !== "") {
				window.scrollTo(0, document.querySelectorAll("#TextContent p")[search].offsetTop);
                        }
		}
	})
}

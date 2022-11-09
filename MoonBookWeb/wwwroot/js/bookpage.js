document.addEventListener("DOMContentLoaded", () => {
	comment = document.querySelector("comment");
	if (!comment) throw "Forum  script: APP not found";
	link = new URL(window.location.href).search.replace('?', "");
	//if link not empty, show choose user
	if (link != '') {
		bookpageLoaded(link);
		loadElement(comment);
		loadRating();
	}
	else {
		alert("Book don't found")
    }
});
let comment;
let link;
let read = document.getElementById("Read");
let follow = document.getElementById("Follow");
let send = document.getElementById("Send");
let commentAnswer = document.querySelector("#CommentAnswerVeiw");
function bookpageLoaded(link) {
	fetch(`/api/BookPage/${link}`, {
		method: "GET",
		body: null
	}).then(r => r.json()).then(j => {
		if (j.status === "Error") {
			alert(j.message)
		} else {
			document.getElementById("Title").innerText = j.message.title
			document.getElementById("Author").textContent += j.message.author
			document.getElementById("Genry").textContent += j.message.genry
			document.getElementById("Date").textContent += j.message.date
			document.getElementById("CoverBook").setAttribute("src", `../img_post/${j.message.coverName}`)
			document.getElementById("Follow").innerHTML = j.follow ? `<img src="../icons/bookmark_remove_FILL0_wght400_GRAD0_opsz48.png"> Remove Library` : `<img src="../icons/bookmark_add_FILL0_wght400_GRAD0_opsz48.png"> Add Library`
			document.querySelector("num").textContent = j.grades
			document.querySelector("#SubBook p").textContent = j.sub;
			document.getElementById("UserRating").textContent = j.rating.length !== 0 ? ((j.rating.map(b => b.grade).reduce((x, y) => x + y, 0)) / j.rating.length).toFixed(1) : 0
        }
    })
}
function loadElement(elem) {
	fetch(`/api/comment/${link}`, {
		method: "GET",
		body: null
	}).then(r => r.json())
		.then(j => {
			if (j.status === "Ok") {
				showElement(elem, j);
			}
			else {
				throw "showTopics: Backend data invalid";
			}
		});
}
function showElement(elem, j) {
	let appHtmlComment = "";
	for (let comment of j.message) {
		var tmpCom = `<div class="CommentUser" id="{{Id}}">
							<div>
								<img id="PhotoComment" src="/img/{{PhotoName}}"/>
							</div>
							<div style="width: 100%;">
								<div id="CommentInfo">
									<a id="CommentAuthor" href="../User/FreandPage?{{Login}}">
										<b>{{Name}} {{Surname}}</b>
									</a>
									<i>{{Date}}</i>
							<div id="Quote" class="Quote">
								{{LinkImg}}
								<a href="{{Link}}">{{Quote}}</a>
							</div>
							</div>
							<div>
								<p class="CommentText" style="color: black;">{{Answer}} {{Text}}</p>
							</div>
							</div>
							<img id="AnswerComment" src="../icons/subdirectory_arrow_right_FILL0_wght400_GRAD0_opsz48.png"/>
							{{EditComment}}
							{{DeleteComment}}
						</div>`;
		tmpCom = tmpCom.replaceAll("{{Login}}", comment.comment.user.login)
			.replaceAll("{{Name}}", comment.comment.user.name)
			.replaceAll("{{Surname}}", comment.comment.user.surname)
			.replaceAll("{{Date}}", comment.comment.comment.date)
			.replaceAll("{{Id}}", comment.comment.comment.id)
			.replaceAll("{{Quote}}", comment.comment.comment.quote === null ? "" : comment.comment.comment.quote)
			.replaceAll("{{LinkImg}}", comment.comment.comment.link === "" || comment.comment.comment.link === null ? "" : `<img src="../icons/subdirectory_arrow_right_FILL0_wght400_GRAD0_opsz48.png"/>`)
			.replaceAll("{{Link}}", comment.comment.comment.link)
			.replaceAll("{{PhotoName}}", (comment.comment.user.photoName === null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : comment.comment.user.photoName))
			.replaceAll("{{Text}}", (comment.comment.comment.text === null ? "" : comment.comment.comment.text))
			.replaceAll("{{DeleteComment}}", (comment.comment.comment.idUser !== j.user ? "" : `<img id="CommentDelete" src="../icons/delete_FILL0_wght400_GRAD0_opsz48.png"/>`))
			.replaceAll("{{EditComment}}", (comment.comment.comment.idUser !== j.user ? "" : `<img id="EditComment" src="../icons/drive_file_rename_outline_FILL0_wght400_GRAD0_opsz48.png"/>`))
			.replaceAll("{{Answer}}", (comment.userAnswer.length === 0 ? "" : `<a href="../User/FreandPage?${comment.userAnswer[0].login}">${comment.userAnswer[0].name} ${comment.userAnswer[0].surname}</a>, `))
		appHtmlComment += tmpCom;
	}
	elem.innerHTML = appHtmlComment;
	commentLoaded();
}
async function commentLoaded() {
	for (let comment of document.querySelectorAll(".CommentUser")) {
		comment.onclick = Comment;
	}
}
function Comment(e) {
	//Delete coment current post
	if (e.target.id === "CommentDelete") {
		let Comment = e.target.closest(".CommentUser");
		let idComment = Comment.getAttribute('id');
		fetch(`/api/comment/${idComment}`, {
			method: "DELETE",
			body: null
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				throw `Comment.Delete: ${j.status}`;
			} else {
				Comment.style.display = `none`;
			}
		})
	}
	//Edit Comment
	if (e.target.id === "EditComment") {
		const post = e.target.closest(".CommentUser");
		let textTmpA = post.querySelector(".CommentText a");
		let text = post.querySelector(".CommentText")
		let idComment = post.getAttribute("id");
		if (text.getAttribute("contenteditable")) {  // alredy begin edit
			e.target.style.color = "orange";
			e.target.style.border = "none";
			text.removeAttribute("contenteditable");
			// check, if content was changet
			if (text.innerText != text.savedContent) {
				// if change
				if (confirm("Save change?")) {
					// if yes, send for server
					fetch(`/api/comment/${idComment}`, {
						method: "PUT",
						headers: {
							"Content-Type": "application/json"
						},
						body: JSON.stringify(textTmpA === null ? text.innerText : text.innerText.replaceAll(textTmpA.innerText + ", ", ""))
					}).then(r => r.json())
						.then(j => {
							if (j.status === "Error") {
								alert(j.message);
								text.innerText = text.savedContent;
							}
						});
				}
			}
		}
		else {  // start change
			e.target.style.color = "lime";
			e.target.style.border = "1px solid yellow";
			text.setAttribute("contenteditable", "true");
			text.savedContent = text.innerText;
			text.focus();
		}
	}
	if (e.target.id === "AnswerComment") {
		let textComment = document.querySelector("#TextComment");
		commentAnswer.querySelector("p").textContent = e.target.previousElementSibling.querySelector("#CommentAuthor").textContent;
		commentAnswer.style.visibility = "visible";
		commentAnswer.querySelector("a").setAttribute("href", e.target.previousElementSibling.querySelector("#CommentAuthor").getAttribute("href"))
		textComment.focus();
	}
}
document.getElementById("CloseAnswer").addEventListener("click", () => {
	commentAnswer.style.visibility = "hidden"
	commentAnswer.querySelector("p").textContent = "";
})
read.addEventListener("click", () => {
	window.location.href = `/Books/UserLibrary?${link}`;
})
follow.addEventListener("click", () => {
	fetch(`/api/book/${link}`,
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
				document.getElementById("Follow").innerHTML = j.message ? `<img src="../icons/bookmark_remove_FILL0_wght400_GRAD0_opsz48.png"> Remove Library` : `<img src="../icons/bookmark_add_FILL0_wght400_GRAD0_opsz48.png"> Add Library`;
				document.querySelector("#SubBook p").textContent = j.sub;
			}
		});
})
send.addEventListener("click", () => {
	let Text = document.getElementById("TextComment");
	let answer;
	try {
		answer = commentAnswer.querySelector("a").getAttribute("href").replace("../User/FreandPage?", "")

	} catch (e) {
		answer = "";
	}
	if (Text.textContent != "") {
		const formData = new FormData();
		formData.append("Text", Text.innerText);
		formData.append("id", link);
		formData.append("Answer", answer)
		fetch("/api/comment", {
			method: "POST",
			body: formData
		}).then(r => r.json()).then(j => {
			if (j.status == "Error") {
				alert(j.message);
			} else {
				Text.textContent = "";
				commentAnswer.style.visibility = "hidden";
				var tmpCom = `<div class="CommentUser" id="{{Id}}"> 
									<div>
										<img id="PhotoComment" src="/img/{{PhotoName}}"/>
									</div>
									<div style="width:100%;">
										<div id="CommentInfo">
											<p>
												<b>{{Name}} {{Surname}}</b>
											</p> 
										<i>{{Date}}</i>
										</div>
										<div>
											<p class="CommentText">{{Answer}} {{Text}}</p>
										</div>
									</div>
									<img id="AnswerComment" src="../icons/subdirectory_arrow_right_FILL0_wght400_GRAD0_opsz48.png"/>
									<img id="EditComment" src="../icons/drive_file_rename_outline_FILL0_wght400_GRAD0_opsz48.png"/>
									<img id="CommentDelete" src="../icons/delete_FILL0_wght400_GRAD0_opsz48.png"/>
								</div>`;
				tmpCom = tmpCom.replaceAll("{{Name}}", j.message.user.name)
					.replaceAll("{{Surname}}", j.message.user.surname)
					.replaceAll("{{Date}}", j.message.comment.date)
					.replaceAll("{{Id}}", j.message.comment.id)
					.replaceAll("{{PhotoName}}", (j.message.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : j.message.user.photoName))
					.replaceAll("{{Text}}", (j.message.comment.text == null ? "" : j.message.comment.text))
					.replaceAll("{{Answer}}", (j.answer === null ? "" : `<a href="../User/FreandPage?${j.answer.login}">${j.answer.name} ${j.answer.surname}</a>, `))
				comment.innerHTML += tmpCom;
            }
		})
    }
})
async function loadRating() {
	for (let radio of document.querySelectorAll("#Rating input[type=radio]")) {
		radio.onclick = Rating;
	}
}
//Text Orintations
function Rating(e) {
	const formData = new FormData();
	formData.append("Grade", e.target.value);
	fetch(`/api/bookpage/${link}`, {
		method: "POST",
		body: formData
	}).then(r => r.json())
		.then(j => {
			if (j.status === "Error") {
				alert(`BookPage.PostGrade: ${j.message}`)
			} else {
				document.querySelector("num").textContent = j.message.length
				document.getElementById("UserRating").textContent = ((j.message.map(b => b.grade).reduce((x, y) => x + y, 0)) / j.message.length).toFixed(1)
			}
		});
}
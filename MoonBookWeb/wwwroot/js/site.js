﻿class Posts {
	constructor(API) {
		this.API = API;
		this.Reactions = this.Reactions.bind(this);
	}

	loadElement(elem) {
		//posts request
		fetch(this.API,
			{
				method: "GET",
				body: null
			})
			.then(r => r.json())
			.then(j => {
				if (j.status === "Ok") {
					//show element
					this.showElement(elem, j, "")
				}
				else {
					throw "showTopics: Backend data invalid";
				}
			});
	}
	showElement(elem, j, appHtml) {
		fetch("/tmpl/post.html")
			.then(r => r.text())
			.then(trTemplate => {
				//replace data posts
				for (let post of j.message) {
					var tmp = trTemplate;
					tmp = tmp.replaceAll("{{Login}}", post.post.post.user.login)
						.replaceAll("{{Name}}", post.post.post.user.name)
						.replaceAll("{{Surname}}", post.post.post.user.surname)
						.replaceAll("{{Date}}", post.post.post.post.date)
						.replaceAll("{{Like}}", post.like)
						.replaceAll("{{Dislike}}", post.dislike)
						.replaceAll("{{Id}}", post.post.post.post.id)
						.replaceAll("{{PhotoUser}}", (post.post.post.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : post.post.post.user.photoName))
						.replaceAll("{{PostImg}}", (post.post.post.post.image == null ? "" : `<img id="PostImg" src="/img_post/${post.post.post.post.image}" />`))
						.replaceAll("{{Text}}", (post.post.post.post.text == null ? "" : post.post.post.post.text))
						.replaceAll("{{Title}}", (post.post.post.post.title == null ? "" : post.post.post.post.title))
						.replaceAll("{{PostDelete}}", (post.post.post.user.id !== j.user ? "" : `<img id="PostDelete" src="../icons/delete_FILL0_wght400_GRAD0_opsz48.png"/>`))
						.replaceAll("{{PostUpdate}}", (post.post.post.user.id !== j.user ? "" : `<img id="PostUpdate" src="../icons/drive_file_rename_outline_FILL0_wght400_GRAD0_opsz48.png"/>`))
					let appHtmlComment = "";
					if (j.user !== null && j.user !== "" && j.user !== undefined) {
						for (let comment of post.post.comment) {
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
								.replaceAll("{{PhotoName}}", (comment.comment.user.photoName === null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : comment.comment.user.photoName))
								.replaceAll("{{Text}}", (comment.comment.comment.text === null ? "" : comment.comment.comment.text))
								.replaceAll("{{DeleteComment}}", (comment.comment.comment.idUser !== j.user ? "" : `<img id="CommentDelete" src="../icons/delete_FILL0_wght400_GRAD0_opsz48.png"/>`))
								.replaceAll("{{EditComment}}", (comment.comment.comment.idUser !== j.user ? "" : `<img id="EditComment" src="../icons/drive_file_rename_outline_FILL0_wght400_GRAD0_opsz48.png"/>`))
								.replaceAll("{{Answer}}", (comment.userAnswer.length === 0 ? "" : `<a href="../User/FreandPage?${comment.userAnswer[0].login}">${comment.userAnswer[0].name} ${comment.userAnswer[0].surname}</a>, `))
							appHtmlComment += tmpCom;
						}
						tmp = tmp.replace("{{Comments}}", appHtmlComment);
					}
					else {
						tmp = tmp.replace("{{Comments}}", `<p style="margin-left: 1%;"><b>You need login.</b></p>`);

                    }
					appHtml += tmp;
				}
				//add in element: <post><post/>
				elem.innerHTML = appHtml;
				this.postLoaded();
			});
	}
	//Add event
	async postLoaded() {
		for (let post of document.querySelectorAll(".Post")) {
			post.onclick = this.Reactions;
		}
	}
	Reactions(e) {
		let idPost = e.currentTarget.getAttribute("id")
		let dislike = e.currentTarget.lastElementChild.firstElementChild.lastElementChild;
		let like = e.currentTarget.lastElementChild.lastElementChild.lastElementChild;
		// Add Reactions
		if (e.target.id === "SadImg" || e.target.id === "SmailImg") {
			const formData = new FormData();
			formData.append("Reaction", (e.target.id === "SadImg"? 2 : 1));
			formData.append("IdPost", idPost);
			fetch("/api/post", {
				method: "PUT",
				body: formData
			}).then(r => r.json()).then(j => {
				if (j.status == "Error") {
					throw "Post.Reactions: reacnion invalid";
				} else {
					dislike.textContent = j.reactDislike;
					like.textContent = j.reactLike;
				}
			})
		}
		//Add Comment current Post
		if (e.target.id === "SendCommentImg") {
			let commentAnswer = e.currentTarget.querySelector("#CommentAnswerVeiw");
			let answer;
			try {
				answer = commentAnswer.querySelector("a").getAttribute("href").replace("../User/FreandPage?", "")

			} catch (e) {
				answer = "";
            }
			let TextComment = e.currentTarget.querySelector("#TextComment");
			if (TextComment.textContent !== "") {
				const formData = new FormData();
				formData.append("Text", TextComment.innerText);
				formData.append("id", idPost);
				formData.append("Answer", answer)
				fetch("/api/comment", {
					method: "POST",
					body: formData
				}).then(r => r.json()).then(j => {
					if (j.status == "Error") {
						throw "Post.Reactions: comment invalid";
					} else {
						commentAnswer.style.visibility = "hidden"
						commentAnswer.querySelector("p").textContent = "";
						let comment = e.target.closest("#PostComment").previousElementSibling.previousElementSibling;
						var tmpCom = `<div class="CommentUser" id="{{Id}}"> 
										 <div>
										 	<img id="PhotoComment" src="/img/{{PhotoName}}"/>
										 </div>
										 <div style="width:100%;">
										 	<div id="CommentInfo">
										 		<a id="CommentAuthor" href="../User/FreandPage?{{Login}}">
													<b>{{Name}} {{Surname}}</b>
												</a> 
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
						tmpCom = tmpCom.replaceAll("{{Login}}", j.message.user.login)
							.replaceAll("{{Name}}", j.message.user.name)
							.replaceAll("{{Surname}}", j.message.user.surname)
							.replaceAll("{{Date}}", j.message.comment.date)
							.replaceAll("{{Id}}", j.message.comment.id)
							.replaceAll("{{PhotoName}}", (j.message.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : j.message.user.photoName))
							.replaceAll("{{Text}}", (j.message.comment.text == null ? "" : j.message.comment.text))
							.replaceAll("{{Answer}}", (j.answer === null ? "" : `<a href="../User/FreandPage?${j.answer.login}">${j.answer.name} ${j.answer.surname}</a>, `))
						comment.innerHTML += tmpCom;
						TextComment.textContent = "";
					}
				})
            }
		}
		//Delete current post
		if (e.target.id === "PostDelete") {
			let idPost = e.currentTarget.getAttribute("id")
			fetch(`/api/post/${idPost}`, {
				method: "DELETE",
				body: null
			}).then(r => r.json()).then(j => {
				if (j.status == "Error") {
					throw `Post.Delete: ${j.status}`;
				} else {
					location.reload();
				}
			})
		}
		//Delete coment current post
		if (e.target.id === "CommentDelete") {
			let Comment = e.target.closest(".CommentUser");
			let idComment = Comment.getAttribute('id');
			fetch(`/api/comment/${idComment}`, {
				method: "DELETE",
				body: null
			}).then(r => r.json()).then(j => {
				if (j.status == "Error") {
					throw `Post.Delete: ${j.status}`;
				} else {
					Comment.style.display = `none`;
				}
			})
		}
		//Edit Post
		if (e.target.id === "PostUpdate") {
			const post = e.target.closest(".Post");
			let text = post.querySelector(".PostText");
			if (text.getAttribute("contenteditable")) {  // alredy begin edit
				e.target.style.color = "orange";
				e.target.style.border = "none";
				text.removeAttribute("contenteditable");
				// check, if content was changet
				if (text.innerText != text.savedContent) {
					// if change
					if (confirm("Save change?")) {
						// if yes, send for server

						fetch(`/api/post/${idPost}`, {
							method: "PUT",
							headers: {
								"Content-Type": "application/json"
							},
							body: JSON.stringify(text.innerText)
						}).then(r => r.json())
							.then(j => {
								console.log(j);
								if (j.status == "Error") {
									alert(j.message);
									p.innerText = p.savedContent;
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
			let textComment = e.currentTarget.querySelector("#TextComment");
			let commentAnswer = e.currentTarget.querySelector("#CommentAnswerVeiw");
			commentAnswer.querySelector("p").textContent = e.target.previousElementSibling.querySelector("#CommentAuthor").textContent;
			commentAnswer.style.visibility = "visible";
			commentAnswer.querySelector("a").setAttribute("href", e.target.previousElementSibling.querySelector("#CommentAuthor").getAttribute("href"))
			textComment.focus();
		}
		if (e.target.id === "CloseAnswer") {
			let commentAnswer = e.currentTarget.querySelector("#CommentAnswerVeiw");
			commentAnswer.style.visibility = "hidden"
			commentAnswer.querySelector("p").textContent = "";
        }
	}
}

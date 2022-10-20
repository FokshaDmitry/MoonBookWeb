class Posts {
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
					tmp = tmp.replaceAll("{{Login}}", post.post.user.login)
						.replaceAll("{{Name}}", post.post.user.name)
						.replaceAll("{{Surname}}", post.post.user.surname)
						.replaceAll("{{Date}}", post.post.post.date)
						.replaceAll("{{Like}}", post.post.post.like)
						.replaceAll("{{Dislike}}", post.post.post.dislike)
						.replaceAll("{{Id}}", post.post.post.id)
						.replaceAll("{{PhotoUser}}", (post.post.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : post.post.user.photoName))
						.replaceAll("{{PostImg}}", (post.post.post.image == null ? "" : `<img id="PostImg" src="/img_post/${post.post.post.image}" />`))
						.replaceAll("{{Text}}", (post.post.post.text == null ? "" : post.post.post.text))
						.replaceAll("{{Title}}", (post.post.post.title == null ? "" : post.post.post.title))
						.replaceAll("{{PostDelete}}", (post.post.user.id !== j.user ? "" : `<img id="PostDelete" src="../icons/delete_FILL0_wght400_GRAD0_opsz48.png"/>`))
						.replaceAll("{{PostUpdate}}", (post.post.user.id !== j.user ? "" : `<img id="PostUpdate" src="../icons/drive_file_rename_outline_FILL0_wght400_GRAD0_opsz48.png"/>`))
					let appHtmlComment = "";
					if (j.user !== null && j.user !== "" && j.user !== undefined) {
						for (let comment of post.comment) {
							var tmpCom = `<div class="CommentUser" id="{{Id}}"> <div> <img id="PhotoComment" src="/img/{{PhotoName}}"/> </div> <div style="width: 100%;"> <div id="CommentInfo"> <a id="CommentAuthor" href="../User/FreandPage?{{Login}}"><b>{{Name}} {{Surname}}</b></a> <i>{{Date}}</i> </div> <div> <p class="CommentText" style="color: black;">{{Text}}</p> </div> </div> <img id="AnswerComment" src="../icons/subdirectory_arrow_right_FILL0_wght400_GRAD0_opsz48.png"/> {{EditComment}} {{DeleteComment}} </div>`;
							tmpCom = tmpCom.replaceAll("{{Login}}", comment.user.login)
								.replaceAll("{{Name}}", comment.user.name)
								.replaceAll("{{Surname}}", comment.user.surname)
								.replaceAll("{{Date}}", comment.comment.date)
								.replaceAll("{{Id}}", comment.comment.id)
								.replaceAll("{{PhotoName}}", (comment.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : comment.user.photoName))
								.replaceAll("{{Text}}", (comment.comment.text == null ? "" : comment.comment.text))
								.replaceAll("{{DeleteComment}}", (comment.comment.idUser !== j.user ? "" : `<img id="CommentDelete" src="../icons/delete_FILL0_wght400_GRAD0_opsz48.png"/>`))
								.replaceAll("{{EditComment}}", (comment.comment.idUser !== j.user ? "" : `<img id="EditComment" src="../icons/drive_file_rename_outline_FILL0_wght400_GRAD0_opsz48.png"/>`))
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
			if (TextComment.value !== "") {
				const formData = new FormData();
				formData.append("Text", TextComment.value);
				formData.append("idPost", idPost);
				formData.append("Answer", answer)
				fetch("/api/post/comment", {
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
										 <div>
										 	<div id="CommentInfo">
										 		<p>
										 			<b>{{Name}} {{Surname}}</b>
										 		</p> <i>{{Date}}</i>
										 	</div>
										 	<div>
										 		<p class="CommentText">{{Text}}</p>
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
						comment.innerHTML += tmpCom;
						TextComment.value = "";
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
			fetch(`/api/post/comment/${idComment}`, {
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
			let text = post.querySelector(".CommentText");
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

						fetch(`/api/post/comment/${idComment}`, {
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
		if (e.target.id === "AnswerComment") {
			let textComment = e.currentTarget.querySelector("#TextComment");
			let commentAnswer = e.currentTarget.querySelector("#CommentAnswerVeiw");
			commentAnswer.querySelector("p").textContent = e.target.previousElementSibling.querySelector("#CommentAuthor").textContent;
			commentAnswer.style.visibility = "visible";
			commentAnswer.querySelector("a").setAttribute("href", e.target.previousElementSibling.querySelector("#CommentAuthor").getAttribute("href"))
			textComment.focus();
			textComment.value = e.target.closest("#CommentAuthor").textContent;
		}
		if (e.target.id === "CloseAnswer") {
			let commentAnswer = e.currentTarget.querySelector("#CommentAnswerVeiw");
			commentAnswer.style.visibility = "hidden"
			commentAnswer.querySelector("p").textContent = "";
        }
	}
}

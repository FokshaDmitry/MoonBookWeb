class Posts {
	constructor(API) {
		this.API = API;
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
					this.showElement(elem, j.message, "")
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
				for (let post of j) {
					var tmp = trTemplate;
					tmp = tmp.replaceAll("{{Name}}", post.post.user.name)
						.replaceAll("{{Surname}}", post.post.user.surname)
						.replaceAll("{{Date}}", post.post.post.date)
						.replaceAll("{{Like}}", post.post.post.like)
						.replaceAll("{{Dislike}}", post.post.post.dislike)
						.replaceAll("{{Id}}", post.post.post.id)
						.replaceAll("{{PhotoUser}}", (post.post.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : post.post.user.photoName))
						.replaceAll("{{PostImg}}", (post.post.post.image == null ? "" : `<img id="PostImg" src="/img_post/${post.post.post.image}" />`))
						.replaceAll("{{Text}}", (post.post.post.text == null ? "" : post.post.post.text))
						.replaceAll("{{Title}}", (post.post.post.title == null ? "" : post.post.post.title))
					let appHtmlComment = "";
					for (let comment of post.comment) {
						var tmpCom = `<div class="CommentUser" id="{{Id}}"> <div> <img id="PhotoComment" src="/img/{{PhotoName}}"/> </div> <div style="width: 100%;"> <div id="CommentInfo"> <p><b>{{Name}} {{Surname}}</b></p> <i>{{Date}}</i> </div> <div> <p style="color: black;">{{Text}}</p> </div> </div> <img id="CommentDelete" src="../icons/delete_FILL0_wght400_GRAD0_opsz48.png"/> </div>`;
						tmpCom = tmpCom.replaceAll("{{Name}}", comment.user.name)
							.replaceAll("{{Surname}}", comment.user.surname)
							.replaceAll("{{Date}}", comment.comment.date)
							.replaceAll("{{Id}}", comment.comment.id)
							.replaceAll("{{PhotoName}}", (comment.user.photoName == null ? "android_contacts_FILL0_wght400_GRAD0_opsz48.png" : comment.user.photoName))
							.replaceAll("{{Text}}", (comment.comment.text == null ? "" : comment.comment.text))
						appHtmlComment += tmpCom;
					}
					tmp = tmp.replace("{{Comments}}", appHtmlComment);
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
		// Add Sad Reactions
		if (e.target.id === "SadImg") {
			const formData = new FormData();
			formData.append("Reaction", 2);
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
		//Add Smail Reactions
		if (e.target.id === "SmailImg") {
			const formData = new FormData();
			formData.append("Reaction", 1);
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
			let TextComment = e.target.parentElement.nextElementSibling;
			if (TextComment.value !== "") {
				const formData = new FormData();
				formData.append("Text", TextComment.value);
				formData.append("idPost", idPost);
				fetch("/api/post/Comment", {
					method: "PUT",
					body: formData
				}).then(r => r.json()).then(j => {
					if (j.status == "Error") {
						throw "Post.Reactions: comment invalid";
					} else {
						let comment = e.target.parentElement.parentElement.previousElementSibling;
						var tmpCom = `<div class="CommentUser" id="{{Id}}"> <div > <img id="PhotoComment" src="/img/{{PhotoName}}"/> </div> <div> <div id="CommentInfo"> <p><b>{{Name}} {{Surname}}</b></p> <i>{{Date}}</i> </div> <div> <p>{{Text}}</p> </div> </div> </div>`;
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
			fetch(`/api/user/${idComment}`, {
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
							method: "POST",
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
	}
}

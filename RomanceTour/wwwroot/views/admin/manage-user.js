function FilterProductCallback(model) {
	$(".user-list").html('');
	$.each(model, function (index, item) {
		$(".user-list").append(`
				<tr id="user-${item.Id}">
					<td class="d-table-cell">
						<span class="table-body">
							${item.Name}
						</span>
					</td>
					<td class="d-none d-md-table-cell">
						<span class="table-body">
							${item.UserName}
						</span>
					</td>
					<td class="d-none d-md-table-cell">
						<span class="table-body">
							${new Date(item.Birthday).toLocaleDateString()}
						</span>
					</td>
					<td class="d-table-cell">
						<span class="table-body">
							${item.Phone}
						</span>
					</td>
					<td class="d-none d-xl-table-cell">
						<span class="table-body">
							${item.Address}
						</span>
					</td>
					<td class="d-table-cell">
						<span class="table-body">
							<button class="btn third config-user">
								<span class="fa fa-user-cog btn-icon"></span>
							</button>
							<button class="btn third remove-user">
								<span class="fa fa-trash btn-icon"></span>
							</button>
						</span>
					</td>
				</tr>
			`);
		$(`#user-${item.Id} .remove-user`).on("click", function () {
			if (confirm("정말로 해당 사용자를 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = $(this).parent().parent().parent().attr("id").split("-")[1];
				Ajax("/User/RemoveUser", {
					id: id
				}, function (model) {
					if (model) alert("사용자가 정상적으로 제거되었습니다.");
					else alert("사용자 제거에 실패하였습니다.");
					Ajax("/User/ListUser", {}, FilterProductCallback);
				});
			}
		});
	});
}

$(document).ready(function () {
	$(".go-back").on("click", function () {
		window.location.href = "/Admin/Dashboard";
	});
	$(".search-bar-second .search-text").on("focus", function () {
		$(this).parent().css("box-shadow", "0px 0px 10px #B0C8EB");
		$(this).parent().css("border", "1px solid #B0C8EB");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#B0C8EB",
			"text-shadow": "0px 0px 10px #B0C8EB"
		});
	});
	$(".search-bar-second .search-text").on("focusout", function () {
		$(this).parent().css("box-shadow", "initial");
		$(this).parent().css("border", "1px solid #B0C8EB");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#B0C8EB",
			"text-shadow": "none"
		});
	});
	$(".search-bar-second .search-text").on("keyup", function () {
		var option = $(".search-option option:selected").val();
		var keyword = $(this).val();
		AjaxWithoutLoading("/User/SearchUser", {
			option: option,
			keyword: keyword
		}, FilterProductCallback);
	});

	Ajax("/User/ListUser", {}, FilterProductCallback);
});
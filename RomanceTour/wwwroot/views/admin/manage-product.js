function FilterProductCallback(model)
{
	$(".product-list").html('');
	$.each(model, function (_index, item)
	{
		$(".product-list").append(`
			<tr id="product-${item.Id}">
				<td class="d-none d-xl-table-cell">
					<span class="table-body">
						<img class="thumbnail-image" src="${item.Thumbnail}" />
					</span>
				</td>
				<td class="d-table-cell">
					<a class="a-suppress" href="/Product/GetProduct?id=${item.Id}">
						<span class="table-body product-title">
							${item.Title}
						</span>
					</a>
				</td>
				<td class="d-table-cell">
					<span class="table-body">
						${item.CategoryName}
					</span>
				</td>
				<td class="d-none d-md-table-cell">
					<span class="table-body">
						&#8361;${item.Price}
					</span>
				</td>
				<td class="d-table-cell">
					<span class="table-body">
						<button class="btn third edit-product">
							<span class="fa fa-edit btn-icon"></span>
						</button>
						<button class="btn third remove-product">
							<span class="fa fa-trash btn-icon"></span>
						</button>
					</span>
				</td>
			</tr>
		`);
		$(`#product-${item.Id} .edit-product`).on("click", function ()
		{
			var id = $(this).parent().parent().parent().attr("id").split("-")[1];
			window.location.href = `/Admin/EditProduct?id=${id}`;
		});
		$(`#product-${item.Id} .remove-product`).on("click", function ()
		{
			if (confirm("정말로 해당 상품을 제거하시겠습니까?\n이 동작은 취소할 수 없습니다."))
			{
				var id = $(this).parent().parent().parent().attr("id").split("-")[1];
				Ajax("/Product/RemoveProduct", {
					id: id
				}, function (model)
				{
					if (model) alert("상품이 정상적으로 제거되었습니다.");
					else alert("상품 제거에 실패하였습니다.");
					RefreshList();
				});
			}
		});
	});
}
function RefreshList()
{
	var category = parseInt($("#category option:selected").val());
	Ajax("/Product/FilterProduct", {
		category: category
	}, FilterProductCallback);
}

$(document).ready(function ()
{
	$(".go-back").on("click", function ()
	{
		window.location.href = "/Admin/Dashboard";
	});
	$(".search-bar-second .search-text").on("focus", function ()
	{
		$(this).parent().css("box-shadow", "0px 0px 10px #D0B8F5");
		$(this).parent().css("border", "1px solid #D0B8F5");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#D0B8F5",
			"text-shadow": "0px 0px 10px #D0B8F5"
		});
	});
	$(".search-bar-second .search-text").on("focusout", function ()
	{
		$(this).parent().css("box-shadow", "initial");
		$(this).parent().css("border", "1px solid #D0B8F5");
		$(this).parent().find(".user-search-btn > .fa-search").css({
			"color": "#D0B8F5",
			"text-shadow": "none"
		});
	});
	$(".write-product").on("click", function ()
	{
		window.location.href = "/Admin/WriteProduct";
	});

	$("#category").on("change", function ()
	{
		RefreshList();
	});
	$("#search-keyword").on("keyup", function ()
	{
		var category = parseInt($("#category option:selected").val());
		var keyword = $(this).val();
		var option = parseInt($("#option").children("option:selected").val());
		switch (option)
		{
			case 0: {
				AjaxWithoutLoading("/Product/FilterProduct", {
					category: category,
					filter: {
						Keyword: keyword
					}
				}, FilterProductCallback);
				break;
			}
			case 1: {
				AjaxWithoutLoading("/Product/FilterProduct", {
					category: category,
					filter: {
						FromPrice: parseInt(keyword),
						ToPrice: 10000000
					}
				}, FilterProductCallback);
				break;
			}
		}
	});

	RefreshList();
});
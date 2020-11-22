var datepicker;
var SCROLL_RATIO = 160;
var DURATION = 300;

$(document).ready(function ()
{
	datepicker = $(".date-picker").datepicker({
		language: "ko",
		todayButton: true
	}).data('datepicker');
	$(".start-date").val(new Date().toDateInputValue());

	$("#product-keyword").on("keyup", function (e)
	{
		if (e.keyCode == 13) $("#search-product").trigger("click");
	});
	$(".day-tour .see-more").on("click", function ()
	{
		window.location.href = "/Product/ListProduct?category=1";
	});
	$(".night-tour .see-more").on("click", function ()
	{
		window.location.href = "/Product/ListProduct?category=2";
	});
	$(".island-tour .see-more").on("click", function ()
	{
		window.location.href = "/Product/ListProduct?category=3";
	});
	$(".jeju-tour .see-more").on("click", function ()
	{
		window.location.href = "/Product/ListProduct?category=4";
	});
	$(".product-item").hover(function ()
	{
		if (!IsMobile) $(this).find(".product-description").stop().slideToggle(DURATION);
	}, function ()
	{
		if (!IsMobile) $(this).find(".product-description").stop().slideToggle(DURATION);
	});
	$(".product-item").on("click", function ()
	{
		var id = $(this).attr("id").split("-")[1];
		window.location.href = `/Product/GetProduct?id=${id}`;
	});
	$("#search-product").on("click", function ()
	{
		var category = $("#product-category").children("option:selected").val();
		var fromDate = datepicker.selectedDates[0] ? datepicker.selectedDates[0].toISOString() : "";
		var keyword = $("#product-keyword").val();

		window.location.href = `/Product/ListProduct?category=${category}&date=${fromDate}&keyword=${keyword}`;
	});
	new Swiper(".swiper-container", {
		direction: "horizontal",
		slidesPerView: "auto",
		navigation: {
			nextEl: ".swiper-button-next",
			prevEl: ".swiper-button-prev",
		}
	});
});
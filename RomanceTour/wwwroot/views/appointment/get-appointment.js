function Initialize() {
	var header = parseFloat($("header").height());
	var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height() + parseFloat($(".collapse").css("paddingTop")) * 2);
	var padding = parseFloat($(".full-screen-area").css("paddingTop"));

	$("article").height(window.innerHeight - header + collapse - (padding * 2));
	$(window).resize(function () {
		var header = parseFloat($("header").height());
		var collapse = $(".collapse").css("display") == "none" || $(window).width() >= 1200 ? 0 : parseFloat($(".collapse").height()) + (parseFloat($(".collapse").css("paddingTop")) * 2);

		var padding = parseFloat($(".full-screen-area").css("paddingTop"));
		$("article").height(window.innerHeight - header + collapse - (padding * 2));
	});
}
function Print() {
	var origin = document.body.innerHTML;
	var printArea = document.getElementById("printable-area").innerHTML;
	document.body.innerHTML = printArea;
	var containers = document.querySelectorAll(".kakao-map-container");
	$.each(containers, function (index, item) {
		var name = item.querySelector(".departure-name").value;
		var latitude = parseFloat(item.querySelector(".latitude").value);
		var longtitude = parseFloat(item.querySelector(".longtitude").value);
		var position = new kakao.maps.LatLng(latitude, longtitude);

		var options = {
			center: position,
			level: 5
		};
		var map = new kakao.maps.Map(item, options);
		var marker = new kakao.maps.Marker({
			position: position
		});
		marker.setMap(map);
		map.setDraggable(false);
		kakao.maps.event.addListener(map, "click", function (e) {
			if (IsMobile) window.open(`kakaomap://look?p=${latitude},${longtitude}`);
			else window.open(`https://map.kakao.com/link/map/${name},${latitude},${longtitude}`);
		});
	});
	window.onafterprint = function () {
		document.body.innerHTML = origin;
		Initialize();

		$(".copy-btn, .copy-text").on("click", function () {
			var id = $("#appointment-id").text();
			var textarea = document.createElement("textarea");
			textarea.value = id;
			document.body.appendChild(textarea);
			textarea.select();
			textarea.setSelectionRange(0, 9999);
			document.execCommand("copy");
			textarea.remove();

			alert("클립보드에 복사되었습니다.");
		});
		$(".go-back").on("click", function () {
			window.location.href = $("#back").val();
		});
		$(".print-btn").on("click", function () {
			Print();
		});
		var containers = document.querySelectorAll(".kakao-map-container");
		$.each(containers, function (index, item) {
			var name = item.querySelector(".departure-name").value;
			var latitude = parseFloat(item.querySelector(".latitude").value);
			var longtitude = parseFloat(item.querySelector(".longtitude").value);
			var position = new kakao.maps.LatLng(latitude, longtitude);

			var options = {
				center: position,
				level: 5
			};
			var map = new kakao.maps.Map(item, options);
			var marker = new kakao.maps.Marker({
				position: position
			});
			marker.setMap(map);
			map.setDraggable(false);
			kakao.maps.event.addListener(map, "click", function (e) {
				if (IsMobile) window.open(`kakaomap://look?p=${latitude},${longtitude}`);
				else window.open(`https://map.kakao.com/link/map/${name},${latitude},${longtitude}`);
			});
		});
	}
	setTimeout(function () {
		window.print();
	}, 100);
}

$(document).ready(function () {
	Initialize();

	$(".copy-btn, .copy-text").on("click", function () {
		var id = $("#appointment-id").text();
		var textarea = document.createElement("textarea");
		textarea.value = id;
		document.body.appendChild(textarea);
		textarea.select();
		textarea.setSelectionRange(0, 9999);
		document.execCommand("copy");
		textarea.remove();

		alert("클립보드에 복사되었습니다.");
	});
	$(".go-back").on("click", function () {
		window.location.href = $("#back").val();
	});
	$(".print-btn").on("click", function () {
		Print();
	});
	var containers = document.querySelectorAll(".kakao-map-container");
	$.each(containers, function (index, item) {
		var name = item.querySelector(".departure-name").value;
		var latitude = parseFloat(item.querySelector(".latitude").value);
		var longtitude = parseFloat(item.querySelector(".longtitude").value);
		var position = new kakao.maps.LatLng(latitude, longtitude);

		var options = {
			center: position,
			level: 5
		};
		var map = new kakao.maps.Map(item, options);
		var marker = new kakao.maps.Marker({
			position: position
		});
		marker.setMap(map);
		map.setDraggable(false);
		kakao.maps.event.addListener(map, "click", function (e) {
			if (IsMobile) window.open(`kakaomap://look?p=${latitude},${longtitude}`);
			else window.open(`https://map.kakao.com/link/map/${name},${latitude},${longtitude}`);
		});
	});
});
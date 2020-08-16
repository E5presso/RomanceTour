var ANIMATION = 1000;
var DELAY = 1000;
var INITIATE = 500;

$(document).ready(function () {
	$("#splash").css("display", "flex");
	setTimeout(function () {
		$(".st0").fadeIn(ANIMATION);
		setTimeout(function () {
			$(".st4").fadeIn(ANIMATION);
			setTimeout(function () {
				$(".splash-title").fadeIn(ANIMATION);
				$(".splash-title").css("display", "flex");
				$(".splash-text").fadeIn(ANIMATION);
				$(".splash-text").css("display", "flex");
				setTimeout(function () {
					$(".splash-logo g").fadeIn(ANIMATION);
					setTimeout(function () {
						$("#splash").fadeOut(ANIMATION);
					}, DELAY);
				}, DELAY);
			}, DELAY);
		}, DELAY);
	}, INITIATE);
});
var core = {
	// init
	init: function () {
		// show alert messge
		$('#validationMessageAlert span.field-validation-error').each(function () {
			if ($(this).text() != '') {
				var msg = $(this).text();
				var type = $(this).attr('data-valmsg-for');
				core.modal.alert(
					'Alert',
					msg,
					null,
					null,
					type
				);
				return;
			}
		});

		// format all datetime data for class lb-date
		$('body .lb-date').each(function () {
			var text = $(this).text();
			return core.formatData.defaultDate(text);
		});

		// round all input number
		$('body .mask-number').each(function () {
			var oldVal = $(this).val();
			$(this).val(core.helpers.input.roundNum(oldVal));
			$(this).val(core.helpers.addCommas($(this).val()));
			$(this).blur(function () {
				var oldVal = $(this).val();
				$(this).val(core.helpers.input.roundNum(oldVal));
				$(this).val(core.helpers.addCommas($(this).val()));
			});
		});

		// register dropdown list
		$('select').each(function () {
			if ($(this).hasClass('custom')) {
				// todo
			} else {
				var id = $(this).attr('id');
				$('#' + id).select2({
					theme: "bootstrap",
				});
			}
		});

		// Disable click outside of bootstrap modal area to close modal
		$('#modal').modal({
			show: false,
			backdrop: 'static',
			keyboard: false  // to prevent closing with Esc button (if you want this too)
		});

		// register event
		$(document).off("blur", "input.datepicker-input")
			.on("blur", "input.datepicker-input", function () {
				var inputVal = $(this).val();
				if (inputVal == '') {
					return;
				}
				if ($.isNumeric(inputVal)) {
					if (inputVal.length == 4) {
						$(this).val(inputVal.substr(0, 1) + '/' + inputVal.substr(1, 1) + '/' + '20' + inputVal.substr(2));
					}
					if (inputVal.length == 5) {
						$(this).val(inputVal.substr(0, 2) + '/' + inputVal.substr(2, 1) + '/' + '20' + inputVal.substr(3));
					}
					if (inputVal.length == 6) {
						$(this).val(inputVal.substr(0, 2) + '/' + inputVal.substr(2, 2) + '/' + '20' + inputVal.substr(4));
					}
					if (inputVal.length >= 7) {
						$(this).val(inputVal.substr(0, 2) + '/' + inputVal.substr(2, 2) + '/' + inputVal.substr(4));
					}
					$(this).trigger("blur");
				}
			});
	},

	initModal: function () {
		$.validator.unobtrusive.parse($('#modal form'));

		$('#modal select').each(function () {
			if ($(this).hasClass('custom')) {
				// todo
			} else {
				var id = $(this).attr('id');
				var width = $(this).width();
				if (width < 200) {
					$('#' + id).select2({
						theme: "bootstrap",
						width: '200px'
					});
				} else {
					$('#' + id).select2({
						theme: "bootstrap",
					});
				}
			}
		});

		// round all unput number
		$('#modal .mask-number').each(function () {
			var oldVal = $(this).val();
			$(this).val(core.helpers.input.roundNum(oldVal));
			$(this).val(core.helpers.addCommas($(this).val()));
			$(this).blur(function () {
				var oldVal = $(this).val();
				$(this).val(core.helpers.input.roundNum(oldVal));
				$(this).val(core.helpers.addCommas($(this).val()));
			});
		});
		if (typeof initDatepicker !== 'undefined' && $.isFunction(initDatepicker)) {
			initDatepicker();
		}
	},
	// modal core
	modal: {
		defaultConfigs: {
			title: 'Confirmation',
			message: 'Are you sure you want to do this?',
			size: '',
			buttons: 'Cancel, Ok'
		},
		resetDefaultSetting: function () {
			$("#modal .modal-dialog").removeClass(function (index, className) {
				return (className.match(/\bmodal-size\S+/g) || []).join(' ');
			});
			$("#btn_cancel").removeClass(function (index, className) {
				return (className.match(/\bbtn-\S+/g) || []).join(' ');
			});
			$("#btn_ok").removeClass(function (index, className) {
				return (className.match(/\bbtn-\S+/g) || []).join(' ');
			});
			$('#modalMsg').removeClass();
		},
		alert: function (title, message, callback, size, type) {
			$('#modal').modal('hide');
			setTimeout(function () {
				core.modal.resetDefaultSetting();
				size = size == undefined ? '' : size
				$('#modal .modal-dialog').addClass(size);
				// set type alert
				if (type != undefined) {
					$('#modalMsg').addClass('text-' + type);
				}
				$('#modalTitle').text(title);
				$('#modal .content').empty();
				$('#modalMsg').html(message);
				$('#btn_cancel').addClass('btn-default');
				$('#btn_ok').addClass('btn-primary');
				if (!$('#btn_cancel').hasClass('hide'))
					$('#btn_cancel').addClass('hide');
				if ($('#modal .modal-footer').hasClass('hide'))
					$('#modal .modal-footer').removeClass('hide');
				$('#modal').modal('show');
				// register event
				$("#modal").off("click", "#btn_ok")
					.on("click", "#btn_ok", function () {
						if (callback != null)
							callback();
					});
			}, 200);
		},
        confirm: function (title, message, okCallback, cancelCallback, size, buttonOkText, buttonCancelText, buttonOkClass, buttonCancelClass) {
            $('#modal').modal('hide');
            setTimeout(function () {
                core.modal.resetDefaultSetting();
                size = size === undefined ? '' : size;
                $('#modal .modal-dialog').addClass(size);
                $('#modalTitle').text(title);
                $('#modal .content').empty();
                $('#modalMsg').html(message);
                $('#btn_cancel').addClass(buttonCancelClass || 'btn-primary');
                $('#btn_ok').addClass(buttonOkClass || 'btn-default');

                if (buttonCancelText) {
                    $('#btn_cancel').text(buttonCancelText);
                }

                if (buttonOkText) {
                    $('#btn_ok').text(buttonOkText);
                }

                if ($('#btn_cancel').hasClass('hide')) {
                    $('#btn_cancel').removeClass('hide');
                }

                if ($('#modal .modal-footer').hasClass('hide')) {
                    $('#modal .modal-footer').removeClass('hide');
                }

                $('#modal').modal('show');
                // register event
                $("#modal").off("click", "#btn_ok")
                    .on("click", "#btn_ok", function () {
                        okCallback();
                    });
                $("#modal").off("click", "#btn_cancel")
                    .on("click", "#btn_cancel", function () {
                        cancelCallback();
                    });
            }, 200);
        },
		document: function (title, html, size, callbackShow) {
			$('#modal').modal('hide');
			$('#modal').on('shown.bs.modal', function (event) {
				core.initModal();
				setTimeout(function () {
					callbackShow();
				}, 200);
				$(this).off('shown.bs.modal');
			});
			setTimeout(function () {
				core.modal.resetDefaultSetting();
				size = size == undefined ? '' : size;
				$('#modal .modal-dialog').addClass(size);
				$('#modalTitle').text(title);
				$('#modalMsg').empty();
				$('#modal .content').empty().html(html);
				if (!$('#modal .modal-footer').hasClass('hide'))
					$('#modal .modal-footer').addClass('hide');
				$('#modal').modal('show');
			}, 200);
		}
	},

	formatData: {
		defaultDate: function (value) {
			if (value === null) {
				return '-';
			}
			var formattedDate = new Date(value);
			var d = formattedDate.getDate();
			var m = formattedDate.getMonth();
			m += 1;  // JavaScript months are 0-11
			var y = formattedDate.getFullYear();

			return (d + "/" + m + "/" + y);
		},
		defaultDecimal: function (value) {
			if (value === undefined || value == null)
				return '-';
			
			var valFormat = core.helpers.input.roundNum(value);
			valFormat = core.helpers.addCommas(valFormat);

			return valFormat;
		}
	},

	helpers: {
		grid: {
            getFiltersById: function (id, params) {
				var gridData = $(id).bootstrapTable('getOptions');
				params.sortName = gridData.sortName == undefined ? params.sort : gridData.sortName;
				params.sortOrder = gridData.sortOrder == undefined ? params.order : gridData.sortOrder;
				params.pageNumber = gridData.pageNumber == undefined ? 1 : gridData.pageNumber;
				params.pageSize = gridData.pageSize == undefined ? params.limit : gridData.pageSize;
				return params;
			}
		},
		input: {
			roundNum: function (num) {
				if (num != null) {
					num = num.toString().replace(/,/g, '');
				}
				if (!$.isNumeric(num)) {
					return num;
				}
				return parseFloat(num).toFixed(2);
			}
		},
		addCommas: function(nStr){
			nStr += '';
			var x = nStr.split('.');
			var x1 = x[0];
			var x2 = x.length > 1 ? '.' + x[1] : '';
			var rgx = /(\d+)(\d{3})/;
			while (rgx.test(x1)) {
				x1 = x1.replace(rgx, '$1' + ',' + '$2');
			}
			return x1 + x2;
		}
	}
};

$(document).ready(function () {
	core.init();
})
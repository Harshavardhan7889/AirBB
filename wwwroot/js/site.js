// ...existing code...
$(function () {
    // Prevent navigation while page and controls initialize
    let navigationAllowed = false;

    // helper to read current values
    function getCurrentValues() {
        return {
            ActiveWhere: $('#activeWhere').length ? $('#activeWhere').val() : (document.body.dataset.activeWhere || 'all'),
            ActiveWhen: $('#daterange').length ? $('#daterange').val() : (document.body.dataset.activeWhen || ''),
            ActiveWho: $('#guestCount').length ? $('#guestCount').val() : (document.body.dataset.activeWho || '1')
        };
    }

    // build URL and navigate — only if allowed or explicitly requested by user
    function navigateWithParams(overrides, forceNavigate = false) {
        if (!navigationAllowed && !forceNavigate) {
            // skip navigation during initialization
            return;
        }

        const cur = getCurrentValues();
        const params = {
            ActiveWhere: overrides && overrides.ActiveWhere !== undefined ? overrides.ActiveWhere : cur.ActiveWhere,
            ActiveWhen: overrides && overrides.ActiveWhen !== undefined ? overrides.ActiveWhen : cur.ActiveWhen,
            ActiveWho:  overrides && overrides.ActiveWho  !== undefined ? overrides.ActiveWho  : cur.ActiveWho
        };

        const q = Object.keys(params)
            .map(k => encodeURIComponent(k) + '=' + encodeURIComponent(params[k] ?? ''))
            .join('&');

        const url = '@Url.Action("Index","Home")' + (q ? '?' + q : '');
        window.location.href = url;
    }

    // daterangepicker init (Bootstrap daterangepicker)
    if ($('#daterange').length) {
        $('#daterange').daterangepicker({
            autoUpdateInput: false,
            minDate: moment(),
            locale: { format: 'MM/DD/YYYY', separator: ' - ', applyLabel: 'Apply', cancelLabel: 'Clear' }
        });

        // set initial value if present (do NOT navigate)
        const initWhen = document.body.dataset.activeWhen || '';
        if (initWhen) $('#daterange').val(initWhen);

        // user applies a range -> navigate (forceNavigate = true)
        $('#daterange').on('apply.daterangepicker', function (ev, picker) {
        const value = picker.startDate.format('MM/DD/YYYY') + ' - ' + picker.endDate.format('MM/DD/YYYY');
        $(this).val(value);
        document.body.dataset.activeWhen = value; // keep dataset in sync
        // do not call navigateWithParams here so page won't reload on Apply
        });

        $('#daterange').on('cancel.daterangepicker', function () {
            $(this).val('');
            document.body.dataset.activeWhen = '';
            // no navigation on cancel either
        });
    }

    // wire up selects AFTER initialization so their change events won't trigger during setup
    $('#activeWhere').on('change', function () {
        // user changed location -> navigate
        navigateWithParams({ ActiveWhere: $(this).val() }, true);
    });

    $('#guestCount').on('change', function () {
        // user changed guest count -> navigate
        navigateWithParams({ ActiveWho: $(this).val() }, true);
    });

    // all initialization finished — allow future automatic navigation
    navigationAllowed = true;
});
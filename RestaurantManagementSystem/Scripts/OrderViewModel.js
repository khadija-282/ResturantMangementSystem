
var orderDetailMapping = {
    'OrderDetail':{
        key: function (orderDetailItem) {
            return ko.utils.unwrapObservable(orderDetailItem.Id);
        },
        create: function (options) {
            return new OrderDetailViewModel(options.data);
        }
    }
};
OrderDetailViewModel = function (data) {
    var self = this;
    ko.mapping.fromJS(data, orderDetailMapping, self);
    self.LineTotal = ko.computed(function () {
        return (self.Quantity() * self.Price()).toFixed(2);
    });

};
ko.extenders.defaultIfNull = function (target, defaultValue) {
    var result = ko.computed({
        read: target,
        write: function (newValue) {
            if (!newValue) {
                target(defaultValue);
            } else {
                target(newValue);
            }
        }
    });

    result(target());

    return result;
};

OrderViewModel = function (data) {    
    var self = this;
    ko.mapping.fromJS(data, orderDetailMapping, self);
    numberOfClicks = ko.observable(0);
    self.incrementClickCounter = function () {
        var previousCount = this.numberOfClicks;
        self.numberOfClicks = previousCount + 1;
    };
    self.save = function () {
            if (self.numberOfClicks > 1) {
                alert('Clicked multiple times.');
                return;
            }
        $.ajax({
            url: "/Order/Create",
            data: ko.toJSON(self),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                alert(data.responseText);
                window.location.href = "/Order/Create";
            },
            error: function (err) {
                alert(err.responseText);
            }
        });
    },
    self.GrandTotal = ko.computed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.OrderDetail(), function (orderDetailItem) {
            total += parseFloat(orderDetailItem.LineTotal());
        });
        return total.toFixed(2);
        });

    self.DiscountValue = ko.observable(self.DiscountValue()).extend({ defaultIfNull: 0 });
    
    self.TotalAfterDiscount = ko.computed(function () {   
        return parseFloat(self.GrandTotal()) - parseFloat(self.DiscountValue());
    });
    self.GST = ko.computed(function () {
        return (parseFloat(16.0 / 100.0) * self.TotalAfterDiscount()).toFixed(2);
    });

    self.NetTotal = ko.computed(function () {
        var total = parseFloat(self.GST()) + parseFloat(self.TotalAfterDiscount());
        return total.toFixed(2);
    });
    self.addLine = function (obj) {
        //alert(1);
        console.log(JSON.parse(obj));
        var objVals = JSON.parse(obj);
        var newItem = new OrderDetailViewModel({
            "Id": null,
            "OrderId": null,
            "ItemId": objVals.ItemId,
            "ItemName": objVals.ItemName,
            "Price": objVals.Price,
            "Quantity": 1,
            "LineTotal": objVals.Price,
            "VAT": 16.0,
            "DiscountType": 'Percent',
            "DiscountValue": 10.0,
            "GrandTotal": 0.0
        });
        self.OrderDetail.push(newItem);
    };
    self.searchKeyUp = function (d, e) {
        if (e.keyCode == 13) {
            console.log('Enter Pressed.');
            if ($('#txtItemCode').is(':focus')) {
                self.addLineByItemCode();                
            }
            else if ($('#txtQuantity').is(':focus')){
                console.log('>>>>>>>>>>');
                $('#txtQuantity').is(':focus');
            }
        }
    };
    self.addLineByItemCode = function (code) {
        //console.log($('#' + code).val());
        $.ajax({
            url: "/Item/GetItemByCode",
            data: JSON.stringify({ code: $('#txtItemCode').val() }),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data.item != null) {
                    var newItem = new OrderDetailViewModel({
                        "Id": null,
                        "OrderId": null,
                        "ItemId": data.item.Id,
                        "ItemName": data.item.Name,
                        "Price": data.item.Price,
                        "Quantity": 1,
                        "LineTotal": data.item.Price,
                        "VAT": 16.0,
                        "DiscountType": 'Percent',
                        "DiscountValue": 10.0,
                        "GrandTotal": 0.0
                    });
                    self.OrderDetail.push(newItem);
                    $('#txtItemCode').val('');
                    $("#lineItemsBody tr:last td input").focus();
                } else {
                    toastr.error("Item Not Found.");
                }
            },
            error: function (err) {
                alert(err);
                console.log(err);
            }
        });
        
    };
    self.deleteLine = function (item) { self.OrderDetail.remove(item) };
};
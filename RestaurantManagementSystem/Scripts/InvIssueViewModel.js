var ObjectState = {
    Unchanged: 0,
    Added: 1,
    Modified: 2,
    Deleted: 3
};
var InvIssueDetailMapping = {
    'listIssueDetail': {
        key: function (item) {
            return ko.utils.unwrapObservable(item.Id);
        },
        create: function (options) {
            return new InvReturnsDetailDto(options.data);
        }
    }
};
InvIssueDetailDto = function (data) {
    var self = this;
    ko.mapping.fromJS(data, InvIssueDetailMapping, self);

    self.flagInvIssueDetailAsEdited = function () {
        console.log(self.ObjectState);
        if (self.ObjectState !== parseInt(ObjectState.Added)) {
            self.objectState = parseInt(ObjectState.Modified);
        }
        return true;
    };
};
InvIssueDto = function (data) {
    var self = this;
    ko.mapping.fromJS(data, {}, self);
    
    if (self.objectState === parseInt(ObjectState.Added)) {
        self.IssueDate = moment(new Date()).format('MM-DD-YYYY');
        self.IssueDate = moment(self.IssueDate());
    }
    else {
        self.IssueDate = moment(new Date(moment(self.IssueDate()))).format('MM-DD-YYYY');// moment();
    }

    self.save = function () {
       
        self.IssueDate = moment(new Date(moment(self.IssueDate))).format('DD-MM-YYYY');
        console.log('Issue Place');
        console.log(self.IssuePlace());
        console.log(ko.toJSON(self));
        $.ajax({
            url: "/InvIssues/SaveAsync",
            data: ko.toJSON(self),
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.result === false) {
                    toastr.warning("Please complete the details to save successfully!");
                }
                if (data.result.ObjectState === parseInt(ObjectState.Deleted)) {
                    toastr.success('Record Deleted!',
                        {
                            timeOut: 10000,
                            fadeOut: 10000
                        });
                    window.location.href = "/InvIssues/Index";
                }
                if (data.result.ObjectState  === parseInt(ObjectState.Added)) {
                    toastr.success('Record Added!');
                    window.location.href = "/InvIssues/Edit/" + data.result.Id;
                }
                else if (data.result.ObjectState === parseInt(ObjectState.Modified)) {
                    toastr.success("Record Updated");

                }
                ko.mapping.fromJS(data.result, self);
            },
            error: function (err) {
                toastr.warning("Error updating database");
            }
        });
    };

    self.flagInvIssueAsEdited = function () {
        console.log(self);
        if (self.ObjectState !== parseInt(ObjectState.Added)) {
            self.ObjectState = parseInt(ObjectState.Modified);
        }
    };
    self.addChildItem = function () {
        var listitems = null;
        var issueNo = self.IssueNo;
        var issueId = self.Id;
        $.ajax({
            url: "/InvIssues/ListItems",
            type: "POST",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (data) {
                listitems = data.result;
            },
            error: function (err) {
                toastr.warning("Error updating database");
            },
            complete: function (data) {

                var item = new InvIssueDetailDto({ Id: 0, InvIssueId: issueId, InventoryItemId: 0, Quantity: 0, ItemName: "", IssueNo: issueNo, ObjectState: parseInt(ObjectState.Added), listItems: listitems });
                self.listIssueDetail.push(item);
                console.log(self.listIssueDetail());
            }
        });

    };
    self.deleteChildItem = function (invIssueitem) {
        self.listIssueDetail.remove(this);
        var id = invIssueitem.Id();
        if (id > 0) {
            self.listIssueDetailsToDelete().push(id);
        }
        console.log(self.listIssueDetailsToDelete());
    };
};
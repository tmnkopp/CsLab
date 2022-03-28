"use strict";

var _person = _interopRequireDefault(require("./models/person"));

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

var person = new _person.default("David", 20);
person.speak();
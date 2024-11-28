"use strict";
// https://github.com/fullstack-build/tslog
// import { Logger } from "tslog";
Object.defineProperty(exports, "__esModule", { value: true });
exports.log = void 0;
// 通用实现方案
var log = {
    info: function (message) {
        console.log("[INFO] ".concat(message));
    },
    error: function (message) {
        console.error("[ERROR] ".concat(message));
    },
    warn: function (message) {
        console.warn("[WARN] ".concat(message));
    },
    fatal: function (error) {
        console.error("[FATAL] ".concat(error.message));
    }
};
exports.log = log;

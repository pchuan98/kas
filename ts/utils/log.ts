// https://github.com/fullstack-build/tslog
// import { Logger } from "tslog";

// const logger = new Logger({ name: "myLogger" });
// logger.silly("I am a silly log.");
// logger.trace("I am a trace log.");
// logger.debug("I am a debug log.");
// logger.info("I am an info log.");
// logger.warn("I am a warn log with a json object:", { foo: "bar" });
// logger.error("I am an error log.");
// logger.fatal(new Error("I am a pretty Error with a stacktrace."));

// 定义 Log 类型
interface ILog {
    /**
     * 通用信息
     * @param message 
     */
    info(message: any): void;

    /**
     * 非预期的结果，但不会整体出错
     * @param message
     */
    warn(message: any): void;

    /**
     * 错误，部分功能无法使用
     * @param message 
     */
    error(message: any): void;

    /**
     * 严重错误，整个功能都无法使用
     * @param error 
     */
    fatal(error: any): void;
}

// 通用实现方案
const log: ILog = {
    info: (message: any) => {
        console.log(`[INFO] ${message}`);
    },
    error: (message: any) => {
        console.error(`[ERROR] ${message}`);
    },
    warn: (message: any) => {
        console.warn(`[WARN] ${message}`);
    },
    fatal: (error: any) => {
        console.error(`[FATAL] ${error.message}`);
    }
};

export { log };
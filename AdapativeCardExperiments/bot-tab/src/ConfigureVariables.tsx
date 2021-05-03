export const getBaseUrl = ():string => {
    return window.location.origin;
}

export const getAppId = (): string => {
    return "14a6686e-e903-4e55-b945-dc2472381849";
}

export class GlobalVars {
    public static initializeCalled: boolean = false;
    public static currentWindow: Window | any;
    public static parentWindow: Window | any;
    public static isFramelessWindow: boolean = false;
    public static childWindow: Window;
}
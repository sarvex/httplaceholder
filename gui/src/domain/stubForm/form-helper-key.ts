export enum FormHelperKey {
  None = "",
  Tenant = "tenant",
  HttpMethod = "httpMethod",
  StatusCode = "statusCode",
  ResponseBody = "responseBody",
  ResponseBodyPlainText = "responseBodyPlainText",
  ResponseBodyJson = "responseBodyJson",
  ResponseBodyXml = "responseBodyXml",
  ResponseBodyHtml = "responseBodyHtml",
  ResponseBodyBase64 = "responseBodyBase64",
  Redirect = "redirect",
  LineEndings = "lineEndings",
  Scenario = "scenario",
  DynamicMode = "dynamicMode",
  Example = "example",
  Path = "path",
  FullPath = "fullPath",
  Query = "query",
  Body = "body",
  Header = "header",
  Form = "form",
  Host = "host",
}

export function getValues(): string[] {
  const enumType = FormHelperKey as any;
  return Object.keys(FormHelperKey).map((key) => enumType[key]);
}

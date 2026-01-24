import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: true,
  application: {
    baseUrl: 'http://localhost:4200/',
    name: 'OPAC',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44396/',
    redirectUri: baseUrl,
    clientId: 'OPAC_App',
    responseType: 'code',
    scope: 'offline_access OPAC',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:44396',
      rootNamespace: 'OPAC',
    },
    OPAC: {
      url: 'https://localhost:44381',
      rootNamespace: 'OPAC',
    },
  },
} as Environment;

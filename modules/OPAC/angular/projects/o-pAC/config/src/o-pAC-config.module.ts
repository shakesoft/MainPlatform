import {makeEnvironmentProviders} from '@angular/core';
import { OPAC_ROUTE_PROVIDERS } from './providers/route.provider';

export function provideOPACConfig() {
  return makeEnvironmentProviders([OPAC_ROUTE_PROVIDERS])
}

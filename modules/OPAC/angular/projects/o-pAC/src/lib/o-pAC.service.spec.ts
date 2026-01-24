import { TestBed } from '@angular/core/testing';
import { OPACService } from './services/o-pAC.service';
import { RestService } from '@abp/ng.core';

describe('OPACService', () => {
  let service: OPACService;
  const mockRestService = jasmine.createSpyObj('RestService', ['request']);
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: RestService,
          useValue: mockRestService,
        },
      ],
    });
    service = TestBed.inject(OPACService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

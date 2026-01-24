import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { OPACComponent } from './components/o-pAC.component';
import { OPACService } from '@o-pAC';
import { of } from 'rxjs';

describe('OPACComponent', () => {
  let component: OPACComponent;
  let fixture: ComponentFixture<OPACComponent>;
  const mockOPACService = jasmine.createSpyObj('OPACService', {
    sample: of([]),
  });
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [OPACComponent],
      providers: [
        {
          provide: OPACService,
          useValue: mockOPACService,
        },
      ],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OPACComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

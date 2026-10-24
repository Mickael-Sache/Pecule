import { TestBed } from '@angular/core/testing';
import { FocusAfterNavigation } from './focus-after-navigation';

describe('FocusAfterNavigation', () => {
  let service: FocusAfterNavigation;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FocusAfterNavigation);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

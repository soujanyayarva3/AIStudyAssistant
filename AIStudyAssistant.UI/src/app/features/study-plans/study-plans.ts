import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef
} from '@angular/core';
import { finalize } from 'rxjs';

import { StudyPlanService } from '../../core/services/study-plan';
import { StudyPlan } from '../../core/models/study-plan';

@Component({
  selector: 'app-study-plans',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './study-plans.html',
  styleUrl: './study-plans.scss'
})
export class StudyPlans implements OnInit {

  private studyPlanService = inject(StudyPlanService);

  private cdr = inject(ChangeDetectorRef);

  studyPlans: StudyPlan[] = [];

  filteredPlans: StudyPlan[] = [];

  showAddModal = false;

  selectedPlanId = 0;

  searchText = '';

  deleteConfirmId: number | null = null;

  submitted = false;

  saving = false;

  deleting = false;

  minDate = '';

  newPlan = {
    title: '',
    description: '',
    dueDate: '',
    dueTime: '',
    status: 'Pending'
  };


  // =====================================================
  // INIT
  // =====================================================

  ngOnInit(): void {

    this.setMinimumDate();

    this.loadStudyPlans();

  }


  // =====================================================
  // MINIMUM DATE
  // =====================================================

  setMinimumDate(): void {

    const now = new Date();

    const year =
      now.getFullYear();

    const month =
      String(
        now.getMonth() + 1
      ).padStart(2, '0');

    const day =
      String(
        now.getDate()
      ).padStart(2, '0');

    this.minDate =
      `${year}-${month}-${day}`;

  }


  // =====================================================
  // LOAD STUDY PLANS
  // =====================================================

  loadStudyPlans(): void {

    console.log(
      'Loading study plans...'
    );

    this.studyPlanService
      .getStudyPlans()
      .subscribe({

        next: (data: StudyPlan[]) => {

          console.log(
            'STUDY PLANS FROM API:',
            data
          );

          this.studyPlans =
            data || [];

          this.filterPlans();

          // Force UI refresh
          this.cdr.detectChanges();

        },

        error: (err) => {

          console.error(
            'STUDY PLAN LOAD ERROR:',
            err
          );

          this.cdr.detectChanges();

        }

      });

  }


  // =====================================================
  // SEARCH
  // =====================================================

  filterPlans(): void {

    const search =
      this.searchText
        .trim()
        .toLowerCase();

    if (!search) {

      this.filteredPlans =
        [...this.studyPlans];

      return;

    }

    this.filteredPlans =
      this.studyPlans.filter(
        plan =>

          (plan.title || '')
            .toLowerCase()
            .includes(search)

          ||

          (plan.description || '')
            .toLowerCase()
            .includes(search)

          ||

          (plan.status || '')
            .toLowerCase()
            .includes(search)

      );

  }


  // =====================================================
  // OPEN ADD FORM
  // =====================================================

  openAddForm(): void {

    console.log(
      'OPENING ADD FORM'
    );

    this.selectedPlanId = 0;

    this.submitted = false;

    this.deleteConfirmId = null;

    this.saving = false;

    this.newPlan = {

      title: '',

      description: '',

      dueDate: '',

      dueTime: '',

      status: 'Pending'

    };

    this.showAddModal = true;

    this.cdr.detectChanges();

  }


  // =====================================================
  // EDIT PLAN
  // =====================================================

  editPlan(plan: StudyPlan): void {

    console.log(
      'EDITING PLAN:',
      plan
    );

    this.selectedPlanId =
      plan.planId;

    this.submitted = false;

    this.deleteConfirmId = null;

    this.saving = false;

    let dateValue = '';

    let timeValue = '';

    if (plan.dueDate) {

      const date =
        new Date(plan.dueDate);

      if (!isNaN(date.getTime())) {

        const year =
          date.getFullYear();

        const month =
          String(
            date.getMonth() + 1
          ).padStart(2, '0');

        const day =
          String(
            date.getDate()
          ).padStart(2, '0');

        const hours =
          String(
            date.getHours()
          ).padStart(2, '0');

        const minutes =
          String(
            date.getMinutes()
          ).padStart(2, '0');

        dateValue =
          `${year}-${month}-${day}`;

        timeValue =
          `${hours}:${minutes}`;

      }

    }

    this.newPlan = {

      title:
        plan.title || '',

      description:
        plan.description || '',

      dueDate:
        dateValue,

      dueTime:
        timeValue,

      status:
        plan.status || 'Pending'

    };

    console.log(
      'EDIT FORM DATA:',
      this.newPlan
    );

    this.showAddModal = true;

    this.cdr.detectChanges();

  }


  // =====================================================
  // SAVE / UPDATE
  // =====================================================

  savePlan(): void {

    // Prevent multiple clicks
    if (this.saving) {

      console.log(
        'SAVE ALREADY IN PROGRESS'
      );

      return;

    }

    this.submitted = true;

    const title =
      this.newPlan.title.trim();

    const description =
      this.newPlan.description.trim();

    const date =
      this.newPlan.dueDate;

    const time =
      this.newPlan.dueTime;


    // =====================================================
    // VALIDATION
    // =====================================================

    if (!title) {

      console.log(
        'TITLE REQUIRED'
      );

      this.cdr.detectChanges();

      return;

    }

    if (!date) {

      console.log(
        'DATE REQUIRED'
      );

      this.cdr.detectChanges();

      return;

    }

    if (!time) {

      console.log(
        'TIME REQUIRED'
      );

      this.cdr.detectChanges();

      return;

    }


    const selectedDate =
      new Date(
        `${date}T${time}`
      );


    if (
      isNaN(
        selectedDate.getTime()
      )
    ) {

      console.log(
        'INVALID DATE/TIME'
      );

      this.cdr.detectChanges();

      return;

    }


    // =====================================================
    // PREVENT PAST DATE / TIME
    // =====================================================

    if (
      selectedDate <= new Date()
    ) {

      console.log(
        'DATE/TIME IS IN THE PAST'
      );

      this.cdr.detectChanges();

      return;

    }


    // =====================================================
    // REQUEST OBJECT
    // =====================================================

    const plan = {

      title: title,

      description: description,

      dueDate:
        selectedDate.toISOString(),

      status:
        this.newPlan.status

    };

    console.log(
      'SENDING PLAN:',
      plan
    );


    // Lock button
    this.saving = true;

    this.cdr.detectChanges();


    // =====================================================
    // UPDATE EXISTING PLAN
    // =====================================================

    if (this.selectedPlanId > 0) {

      const planId =
        this.selectedPlanId;

      console.log(
        'UPDATING PLAN:',
        planId
      );

      this.studyPlanService
        .updateStudyPlan(
          planId,
          plan
        )
        .pipe(

          finalize(() => {

            // Unlock update button
            this.saving = false;

            // Force immediate UI update
            this.cdr.detectChanges();

            console.log(
              'UPDATE REQUEST FINISHED'
            );

          })

        )
        .subscribe({

          next: () => {

            console.log(
              'PLAN UPDATED SUCCESSFULLY'
            );

            // Close modal
            this.showAddModal = false;

            // Reset values
            this.selectedPlanId = 0;

            this.submitted = false;

            this.newPlan = {

              title: '',

              description: '',

              dueDate: '',

              dueTime: '',

              status: 'Pending'

            };

            // Force UI refresh
            this.cdr.detectChanges();

            // Reload plans
            this.loadStudyPlans();

          },

          error: (err) => {

            console.error(
              'UPDATE ERROR:',
              err
            );

            this.saving = false;

            this.cdr.detectChanges();

          }

        });

      return;

    }


    // =====================================================
    // CREATE NEW PLAN
    // =====================================================

    console.log(
      'CREATING NEW STUDY PLAN'
    );

    this.studyPlanService
      .createStudyPlan(plan)
      .pipe(

        finalize(() => {

          // Unlock save button
          this.saving = false;

          // Force immediate UI update
          this.cdr.detectChanges();

          console.log(
            'CREATE REQUEST FINISHED'
          );

        })

      )
      .subscribe({

        next: (response) => {

          console.log(
            'PLAN CREATED SUCCESSFULLY:',
            response
          );


          // =================================================
          // RESET SAVING
          // =================================================

          this.saving = false;


          // =================================================
          // CLOSE MODAL
          // =================================================

          this.showAddModal = false;


          // =================================================
          // RESET SELECTED PLAN
          // =================================================

          this.selectedPlanId = 0;


          // =================================================
          // RESET VALIDATION
          // =================================================

          this.submitted = false;


          // =================================================
          // CLEAR FORM
          // =================================================

          this.newPlan = {

            title: '',

            description: '',

            dueDate: '',

            dueTime: '',

            status: 'Pending'

          };


          // =================================================
          // FORCE UI UPDATE
          // =================================================

          this.cdr.detectChanges();


          // =================================================
          // RELOAD STUDY PLANS
          // =================================================

          this.loadStudyPlans();


          console.log(
            'MODAL CLOSED - SAVING RESET'
          );

        },

        error: (err) => {

          console.error(
            'CREATE ERROR:',
            err
          );

          this.saving = false;

          this.cdr.detectChanges();

        }

      });

  }


  // =====================================================
  // CLOSE FORM
  // =====================================================

  closeForm(): void {

    console.log(
      'CLOSING STUDY PLAN FORM'
    );

    this.showAddModal = false;

    this.selectedPlanId = 0;

    this.submitted = false;

    this.saving = false;

    this.newPlan = {

      title: '',

      description: '',

      dueDate: '',

      dueTime: '',

      status: 'Pending'

    };

    this.cdr.detectChanges();

  }


  // =====================================================
  // DELETE - OPEN CONFIRMATION
  // =====================================================

  askDelete(id: number): void {

    console.log(
      'DELETE REQUESTED:',
      id
    );

    this.deleteConfirmId = id;

    this.deleting = false;

    this.cdr.detectChanges();

  }


  // =====================================================
  // CANCEL DELETE
  // =====================================================

  cancelDelete(): void {

    this.deleteConfirmId = null;

    this.deleting = false;

    this.cdr.detectChanges();

  }


  // =====================================================
  // CONFIRM DELETE
  // =====================================================

  confirmDelete(): void {

    if (
      this.deleteConfirmId === null
    ) {

      return;

    }


    // Prevent duplicate delete
    if (this.deleting) {

      console.log(
        'DELETE ALREADY IN PROGRESS'
      );

      return;

    }


    const id =
      this.deleteConfirmId;

    console.log(
      'CONFIRMING DELETE:',
      id
    );


    // Lock delete button
    this.deleting = true;

    this.cdr.detectChanges();


    this.studyPlanService
      .deleteStudyPlan(id)
      .pipe(

        finalize(() => {

          // Unlock delete button
          this.deleting = false;

          // Force immediate UI update
          this.cdr.detectChanges();

          console.log(
            'DELETE REQUEST FINISHED'
          );

        })

      )
      .subscribe({

        next: () => {

          console.log(
            'PLAN DELETED SUCCESSFULLY'
          );


          // Remove confirmation
          this.deleteConfirmId = null;


          // Make sure deleting is reset
          this.deleting = false;


          // Force UI update
          this.cdr.detectChanges();


          // Reload plans
          this.loadStudyPlans();

        },

        error: (err) => {

          console.error(
            'DELETE STUDY PLAN ERROR:',
            err
          );

          this.deleting = false;

          this.cdr.detectChanges();

        }

      });

  }

}

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
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

  showAddModal = false;

  selectedPlanId = 0;

  newPlan = {
  taskName: '',
  dueDate: '',
  status: 'Pending'
};
  ngOnInit(): void {
    this.loadStudyPlans();
  }

  loadStudyPlans() {

  console.log("Loading study plans...");

  this.studyPlanService.getStudyPlans().subscribe({

    next: (data: StudyPlan[]) => {

      console.log("API DATA RECEIVED:", data);

      this.studyPlans = [...data];

console.log("studyPlans length:", this.studyPlans.length);

this.cdr.detectChanges();
    },

    error: (err) => {
      console.error(err);
    }

  });

}
  savePlan() {

    if (!this.newPlan.taskName || !this.newPlan.dueDate) {
      alert('Please fill all fields');
      return;
    }

    const plan = {
  taskName: this.newPlan.taskName,
  dueDate: new Date(this.newPlan.dueDate).toISOString(),
  status: this.newPlan.status
};

    this.studyPlanService.createStudyPlan(plan).subscribe({
      next: () => {
        alert('Study Plan Added Successfully');

        this.showAddModal = false;

        this.newPlan = {
          taskName: '',
          dueDate: '',
          status: 'Pending'
        };

        this.loadStudyPlans();
      },
      error: (err: any) => {
        console.error(err);
        alert('Failed to add Study Plan');
      }
    });
  }

  editPlan(plan: StudyPlan) {
    this.selectedPlanId = plan.planId;

   this.newPlan = {
  taskName: plan.taskName,
  dueDate: plan.dueDate,
  status: plan.status
};
    this.showAddModal = true;
  }

  deletePlan(id: number) {

  const confirmDelete = confirm(
    'Are you sure you want to delete this study plan?'
  );


  if (!confirmDelete) {
    return;
  }


  this.studyPlanService.deleteStudyPlan(id)
    .subscribe({

      next: () => {

        alert('Study Plan deleted successfully');

        this.loadStudyPlans();

      },


      error: (err) => {

        console.error(err);

        alert('Failed to delete study plan');

      }

    });

}
}
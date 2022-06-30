/******************************************************************************
 * File: XRITSampleController.cs
 * Copyright (c) 2021-2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class XRITSampleController : SampleController
    {
        public Text ButtonPressText;
        public Text ScrollbarText;

        public void OnButtonPress(string buttonName) {
            ButtonPressText.text = buttonName;
        }

        public void OnScrollValueChanged(float value) {
            ScrollbarText.text = value.ToString("#0.00");
        }
    }
}
